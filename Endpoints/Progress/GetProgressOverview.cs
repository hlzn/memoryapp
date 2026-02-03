using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using MemoryApp.Services;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Progress;

public class GetProgressOverview : Endpoint<GetProgressRequest, ProgressOverviewResponse>
{
    public AppDbContext Db { get; set; } = null!;
    public UserService UserService { get; set; } = null!;

    public override void Configure()
    {
        Get("/api/v1/progress/overview");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetProgressRequest req, CancellationToken ct)
    {
        var user = await UserService.GetOrCreateDefaultUserAsync();
        var level = req.Level ?? "A1";

        // Get total verbs for level
        var totalVerbs = await Db.Verbs.CountAsync(v => v.Level == level, ct);

        // Get verb IDs for this level
        var verbIds = await Db.Verbs
            .Where(v => v.Level == level)
            .Select(v => v.Id)
            .ToListAsync(ct);

        // Get progress for these verbs
        var progressList = await Db.UserVerbProgress
            .Where(p => p.UserId == user.Id && verbIds.Contains(p.VerbId))
            .ToListAsync(ct);

        var masteredVerbs = progressList.Count(p => p.Mastery >= 80);
        var learningVerbs = progressList.Count(p => p.Mastery > 0 && p.Mastery < 80);
        var newVerbs = totalVerbs - progressList.Count;

        var averageMastery = progressList.Any()
            ? progressList.Average(p => p.Mastery)
            : 0;

        // Get attempts stats
        var attempts = await Db.Attempts
            .Where(a => a.UserId == user.Id && a.Mode == "cards" && verbIds.Contains(a.ItemId))
            .ToListAsync(ct);

        var totalAttempts = attempts.Count;
        var correctAttempts = attempts.Count(a => a.IsCorrect);
        var accuracyRate = totalAttempts > 0 ? (double)correctAttempts / totalAttempts * 100 : 0;

        Response = new ProgressOverviewResponse
        {
            Level = level,
            TotalVerbs = totalVerbs,
            MasteredVerbs = masteredVerbs,
            LearningVerbs = learningVerbs,
            NewVerbs = newVerbs,
            AverageMastery = Math.Round(averageMastery, 1),
            TotalAttempts = totalAttempts,
            CorrectAttempts = correctAttempts,
            AccuracyRate = Math.Round(accuracyRate, 1)
        };
    }
}
