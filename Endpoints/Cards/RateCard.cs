using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using MemoryApp.Data.Models;
using MemoryApp.Services;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Cards;

public class RateCard : Endpoint<RateCardRequest, RateCardResponse>
{
    public AppDbContext Db { get; set; } = null!;
    public UserService UserService { get; set; } = null!;
    public SrsService SrsService { get; set; } = null!;

    public override void Configure()
    {
        Post("/api/v1/modes/cards/rate");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RateCardRequest req, CancellationToken ct)
    {
        var user = await UserService.GetOrCreateDefaultUserAsync();

        // Parse rating
        var reviewResult = req.Rating.ToLower() switch
        {
            "know" => ReviewResult.Know,
            "doubt" => ReviewResult.Doubt,
            "dontknow" => ReviewResult.DontKnow,
            _ => ReviewResult.Doubt
        };

        // Get or create progress
        var progress = await Db.UserVerbProgress
            .FirstOrDefaultAsync(p => p.UserId == user.Id && p.VerbId == req.VerbId, ct);

        TimeSpan currentInterval = TimeSpan.Zero;
        int currentMastery = 0;

        if (progress != null)
        {
            currentInterval = progress.NextReviewAt - DateTime.UtcNow;
            if (currentInterval < TimeSpan.Zero) currentInterval = TimeSpan.FromDays(1);
            currentMastery = progress.Mastery;
        }

        // Calculate new values using SRS
        var (masteryDelta, nextInterval) = SrsService.CalculateReview(
            currentMastery,
            currentInterval,
            reviewResult);

        var newMastery = SrsService.ClampMastery(currentMastery + masteryDelta);
        var nextReviewAt = DateTime.UtcNow.Add(nextInterval);

        if (progress == null)
        {
            progress = new UserVerbProgress
            {
                UserId = user.Id,
                VerbId = req.VerbId,
                Mastery = newMastery,
                NextReviewAt = nextReviewAt,
                Streak = reviewResult == ReviewResult.Know ? 1 : 0,
                LastResult = req.Rating
            };
            Db.UserVerbProgress.Add(progress);
        }
        else
        {
            progress.Mastery = newMastery;
            progress.NextReviewAt = nextReviewAt;
            progress.LastResult = req.Rating;
            progress.Streak = reviewResult == ReviewResult.Know ? progress.Streak + 1 : 0;
        }

        // Log attempt
        Db.Attempts.Add(new Attempt
        {
            UserId = user.Id,
            Mode = "cards",
            ItemType = "verb",
            ItemId = req.VerbId,
            IsCorrect = reviewResult == ReviewResult.Know,
            ScoreDelta = masteryDelta,
            CreatedAt = DateTime.UtcNow
        });

        await Db.SaveChangesAsync(ct);

        Response = new RateCardResponse
        {
            Success = true,
            NewMastery = newMastery,
            NextReviewAt = nextReviewAt,
            Message = reviewResult switch
            {
                ReviewResult.Know => $"¡Excelente! Dominio: {newMastery}%",
                ReviewResult.Doubt => $"Bien, pero repasa más. Dominio: {newMastery}%",
                ReviewResult.DontKnow => $"Sigue practicando. Dominio: {newMastery}%",
                _ => ""
            }
        };
    }
}
