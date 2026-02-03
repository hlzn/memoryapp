using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using MemoryApp.Services;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Cards;

public class GetCardsSession : Endpoint<GetCardsSessionRequest, GetCardsSessionResponse>
{
    public AppDbContext Db { get; set; } = null!;
    public UserService UserService { get; set; } = null!;

    public override void Configure()
    {
        Get("/api/v1/modes/cards/session");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetCardsSessionRequest req, CancellationToken ct)
    {
        var user = await UserService.GetOrCreateDefaultUserAsync();

        // Get all verbs for the level
        var verbsQuery = Db.Verbs.Where(v => v.Level == req.Level);
        var totalInLevel = await verbsQuery.CountAsync(ct);

        // Get user progress for these verbs
        var verbIds = await verbsQuery.Select(v => v.Id).ToListAsync(ct);
        var progressMap = await Db.UserVerbProgress
            .Where(p => p.UserId == user.Id && verbIds.Contains(p.VerbId))
            .ToDictionaryAsync(p => p.VerbId, ct);

        List<CardDto> cards;

        if (req.Type == "review")
        {
            // Get verbs due for review
            var now = DateTime.UtcNow;
            var dueVerbs = await Db.Verbs
                .Where(v => v.Level == req.Level)
                .GroupJoin(
                    Db.UserVerbProgress.Where(p => p.UserId == user.Id),
                    v => v.Id,
                    p => p.VerbId,
                    (v, progress) => new { Verb = v, Progress = progress.FirstOrDefault() })
                .Where(x => x.Progress == null || x.Progress.NextReviewAt <= now)
                .OrderBy(x => x.Progress == null ? DateTime.MinValue : x.Progress.NextReviewAt)
                .Take(req.Count)
                .Select(x => new CardDto
                {
                    VerbId = x.Verb.Id,
                    Infinitive = x.Verb.Infinitive,
                    TranslationEs = x.Verb.TranslationEs,
                    PartizipII = x.Verb.PartizipII,
                    Auxiliary = x.Verb.Auxiliary,
                    IsSeparable = x.Verb.IsSeparable,
                    Prefix = x.Verb.Prefix,
                    Notes = x.Verb.Notes,
                    CurrentMastery = x.Progress != null ? x.Progress.Mastery : 0
                })
                .ToListAsync(ct);

            cards = dueVerbs;
        }
        else if (req.Type == "new")
        {
            // Get verbs never practiced
            var practicedVerbIds = await Db.UserVerbProgress
                .Where(p => p.UserId == user.Id)
                .Select(p => p.VerbId)
                .ToListAsync(ct);

            cards = await Db.Verbs
                .Where(v => v.Level == req.Level && !practicedVerbIds.Contains(v.Id))
                .Take(req.Count)
                .Select(v => new CardDto
                {
                    VerbId = v.Id,
                    Infinitive = v.Infinitive,
                    TranslationEs = v.TranslationEs,
                    PartizipII = v.PartizipII,
                    Auxiliary = v.Auxiliary,
                    IsSeparable = v.IsSeparable,
                    Prefix = v.Prefix,
                    Notes = v.Notes,
                    CurrentMastery = 0
                })
                .ToListAsync(ct);
        }
        else // "failed"
        {
            // Get verbs with low mastery
            cards = await Db.UserVerbProgress
                .Where(p => p.UserId == user.Id && p.Mastery < 50)
                .Join(Db.Verbs.Where(v => v.Level == req.Level),
                    p => p.VerbId,
                    v => v.Id,
                    (p, v) => new CardDto
                    {
                        VerbId = v.Id,
                        Infinitive = v.Infinitive,
                        TranslationEs = v.TranslationEs,
                        PartizipII = v.PartizipII,
                        Auxiliary = v.Auxiliary,
                        IsSeparable = v.IsSeparable,
                        Prefix = v.Prefix,
                        Notes = v.Notes,
                        CurrentMastery = p.Mastery
                    })
                .OrderBy(c => c.CurrentMastery)
                .Take(req.Count)
                .ToListAsync(ct);
        }

        Response = new GetCardsSessionResponse
        {
            SessionId = Guid.NewGuid(),
            Cards = cards,
            TotalInLevel = totalInLevel
        };
    }
}
