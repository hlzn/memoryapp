using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using MemoryApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Nouns;

public class CreateNoun : Endpoint<CreateNounRequest, CreateNounResponse>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Post("/api/v1/nouns");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateNounRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.German))
        {
            Response = new CreateNounResponse { Success = false, Error = "German word is required" };
            return;
        }

        var exists = await Db.Nouns.AnyAsync(n => n.German == req.German, ct);
        if (exists)
        {
            Response = new CreateNounResponse { Success = false, Error = $"Noun '{req.German}' already exists" };
            return;
        }

        var article = req.Article?.Trim().ToLower() ?? string.Empty;

        var noun = new Noun
        {
            German = req.German.Trim(),
            Article = article,
            TranslationEs = req.TranslationEs.Trim(),
            Level = req.Level.Trim(),
            Plural = req.Plural?.Trim(),
            Notes = req.Notes?.Trim()
        };

        Db.Nouns.Add(noun);
        await Db.SaveChangesAsync(ct);

        Response = new CreateNounResponse { Success = true, NounId = noun.Id };
    }
}
