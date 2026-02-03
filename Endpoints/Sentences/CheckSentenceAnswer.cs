using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using MemoryApp.Data.Models;
using MemoryApp.Services;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Sentences;

public class CheckSentenceAnswer : Endpoint<CheckSentenceAnswerRequest, CheckSentenceAnswerResponse>
{
    public AppDbContext Db { get; set; } = null!;
    public UserService UserService { get; set; } = null!;

    public override void Configure()
    {
        Post("/api/v1/modes/sentences/answer");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CheckSentenceAnswerRequest req, CancellationToken ct)
    {
        var user = await UserService.GetOrCreateDefaultUserAsync();

        var sentence = await Db.Sentences
            .Include(s => s.Verb)
            .FirstOrDefaultAsync(s => s.Id == req.SentenceId, ct);

        if (sentence == null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        // Normalize answers for comparison (trim + case-insensitive)
        var userAnswer = req.Answer.Trim().ToLower();
        var correctAnswer = sentence.CorrectAnswer.Trim().ToLower();

        bool isCorrect = userAnswer == correctAnswer;

        // Generate explanation based on sentence type
        string? explanation = null;
        if (!isCorrect)
        {
            explanation = sentence.Type switch
            {
                "present" => $"Conjugación presente: {sentence.Verb?.Infinitive} → {sentence.CorrectAnswer}",
                "perfekt" => $"Perfekt con auxiliar {sentence.Verb?.Auxiliary}",
                "separable" => $"Verbo separable: {sentence.Verb?.Infinitive}",
                "preposition" => $"Preposición común con este verbo: {sentence.CorrectAnswer}",
                "infinitive" => $"Forma infinitiva del verbo",
                _ => sentence.Hint
            };
        }

        // Log attempt
        Db.Attempts.Add(new Attempt
        {
            UserId = user.Id,
            Mode = "sentences",
            ItemType = "sentence",
            ItemId = sentence.Id,
            IsCorrect = isCorrect,
            ScoreDelta = isCorrect ? 5 : -2,
            CreatedAt = DateTime.UtcNow
        });

        await Db.SaveChangesAsync(ct);

        Response = new CheckSentenceAnswerResponse
        {
            IsCorrect = isCorrect,
            CorrectAnswer = sentence.CorrectAnswer,
            Explanation = explanation ?? sentence.Hint,
            Message = isCorrect
                ? "¡Correcto! Muy bien."
                : $"Incorrecto. La respuesta correcta es: {sentence.CorrectAnswer}"
        };
    }
}
