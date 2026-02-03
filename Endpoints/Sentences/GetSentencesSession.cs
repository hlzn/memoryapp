using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Sentences;

public class GetSentencesSession : Endpoint<GetSentencesSessionRequest, GetSentencesSessionResponse>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Get("/api/v1/modes/sentences/session");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetSentencesSessionRequest req, CancellationToken ct)
    {
        var query = Db.Sentences.Where(s => s.Level == req.Level);

        // Filter by difficulty
        if (req.Difficulty == "easy")
        {
            // Easy: infinitive and simple present
            query = query.Where(s => s.Type == "infinitive" || s.Type == "present");
        }
        else if (req.Difficulty == "medium")
        {
            // Medium: conjugation without word bank
            query = query.Where(s => s.Type == "present" || s.Type == "preposition");
        }
        else // hard
        {
            // Hard: complex tenses, separable verbs, prepositions
            query = query.Where(s => s.Type == "perfekt" || s.Type == "separable" || s.Type == "preposition");
        }

        // Load sentences to memory first, then randomize
        var allSentences = await query
            .Include(s => s.Verb)
            .ToListAsync(ct);

        var sentences = allSentences
            .OrderBy(x => Guid.NewGuid())
            .Take(req.Count)
            .ToList();

        var questions = sentences.Select(s => new SentenceQuestionDto
        {
            SentenceId = s.Id,
            SentenceTemplate = s.SentenceTemplate,
            TranslationEs = s.TranslationEs,
            Hint = s.Hint,
            Type = s.Type,
            WordBank = req.Difficulty == "easy" && s.Verb != null
                ? new List<string> { s.Verb.Infinitive, s.CorrectAnswer }
                : null
        }).ToList();

        Response = new GetSentencesSessionResponse
        {
            SessionId = Guid.NewGuid(),
            Questions = questions
        };
    }
}
