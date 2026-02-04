using MemoryApp.Contracts;
using MemoryApp.Data;
using MemoryApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Services;

public class MatchingAnswerService
{
    private readonly AppDbContext _db;
    private readonly UserService _userService;
    private readonly SrsService _srsService;
    private readonly AiFeedbackService _aiFeedbackService;

    public MatchingAnswerService(
        AppDbContext db,
        UserService userService,
        SrsService srsService,
        AiFeedbackService aiFeedbackService)
    {
        _db = db;
        _userService = userService;
        _srsService = srsService;
        _aiFeedbackService = aiFeedbackService;
    }

    public async Task<SubmitMatchingAnswerResponse> SubmitAsync(SubmitMatchingAnswerRequest req, CancellationToken ct = default)
    {
        var user = await _userService.GetOrCreateDefaultUserAsync();

        var noun = await _db.Nouns.FindAsync(new object[] { req.NounId }, ct);
        if (noun == null)
        {
            return new SubmitMatchingAnswerResponse { Success = false, Message = "Noun not found" };
        }

        var submittedGerman = req.SubmittedGerman.Trim();
        var submittedArticle = (req.SubmittedArticle ?? string.Empty).Trim();
        var germanCorrect = string.Equals(submittedGerman, noun.German, StringComparison.OrdinalIgnoreCase);
        var articleCorrect = string.Equals(submittedArticle, noun.Article, StringComparison.OrdinalIgnoreCase);

        string result;
        ReviewResult reviewResult;

        if (germanCorrect && articleCorrect)
        {
            result = "correct";
            reviewResult = ReviewResult.Know;
        }
        else if (germanCorrect || articleCorrect)
        {
            result = "partial";
            reviewResult = ReviewResult.Doubt;
        }
        else
        {
            result = "wrong";
            reviewResult = ReviewResult.DontKnow;
        }

        var progress = await _db.UserNounProgress
            .FirstOrDefaultAsync(p => p.UserId == user.Id && p.NounId == req.NounId, ct);

        TimeSpan currentInterval = TimeSpan.Zero;
        int currentMastery = 0;

        if (progress != null)
        {
            currentInterval = progress.NextReviewAt - DateTime.UtcNow;
            if (currentInterval < TimeSpan.Zero) currentInterval = TimeSpan.FromDays(1);
            currentMastery = progress.Mastery;
        }

        var (masteryDelta, nextInterval) = _srsService.CalculateReview(
            currentMastery,
            currentInterval,
            reviewResult);

        var newMastery = _srsService.ClampMastery(currentMastery + masteryDelta);
        var nextReviewAt = DateTime.UtcNow.Add(nextInterval);

        if (progress == null)
        {
            progress = new UserNounProgress
            {
                UserId = user.Id,
                NounId = req.NounId,
                Mastery = newMastery,
                NextReviewAt = nextReviewAt,
                Streak = result == "correct" ? 1 : 0,
                LastResult = result
            };
            _db.UserNounProgress.Add(progress);
        }
        else
        {
            progress.Mastery = newMastery;
            progress.NextReviewAt = nextReviewAt;
            progress.LastResult = result;
            progress.Streak = result == "correct" ? progress.Streak + 1 : 0;
        }

        _db.Attempts.Add(new Attempt
        {
            UserId = user.Id,
            Mode = "matching",
            ItemType = "noun",
            ItemId = req.NounId,
            IsCorrect = result == "correct",
            ScoreDelta = masteryDelta,
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);

        var correctPhrase = string.IsNullOrWhiteSpace(noun.Article)
            ? noun.German
            : $"{noun.Article} {noun.German}";

        var message = result switch
        {
            "correct" => $"¡Perfecto! Dominio: {newMastery}%",
            "partial" => germanCorrect
                ? string.IsNullOrWhiteSpace(noun.Article)
                    ? $"¡Palabra correcta! Esta palabra no lleva artículo. Dominio: {newMastery}%"
                    : $"¡Palabra correcta! Pero el artículo es '{noun.Article}'. Dominio: {newMastery}%"
                : string.IsNullOrWhiteSpace(noun.Article)
                    ? $"Sin artículo era correcto, pero la palabra es '{noun.German}'. Dominio: {newMastery}%"
                    : $"¡Artículo correcto! Pero la palabra es '{noun.German}'. Dominio: {newMastery}%",
            _ => $"Incorrecto. Es '{correctPhrase}'. Dominio: {newMastery}%"
        };

        string? aiComment = null;
        if (result != "correct")
        {
            aiComment = await _aiFeedbackService.GetFeedbackAsync(
                noun.German,
                noun.Article,
                req.SubmittedGerman,
                req.SubmittedArticle,
                noun.TranslationEs,
                req.PreviousResponses,
                ct);
        }

        return new SubmitMatchingAnswerResponse
        {
            Success = true,
            Result = result,
            CorrectGerman = noun.German,
            CorrectArticle = noun.Article,
            NewMastery = newMastery,
            Message = message,
            AiComment = aiComment
        };
    }
}
