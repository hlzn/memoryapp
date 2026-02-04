using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Nouns;

public class UpdateNoun : Endpoint<UpdateNounRequest, UpdateNounResponse>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Put("/api/v1/nouns");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateNounRequest req, CancellationToken ct)
    {
        var noun = await Db.Nouns.FindAsync(new object[] { req.Id }, ct);
        if (noun == null)
        {
            Response = new UpdateNounResponse { Success = false, Error = "Noun not found" };
            return;
        }

        var duplicateExists = await Db.Nouns
            .AnyAsync(n => n.German == req.German && n.Id != req.Id, ct);
        if (duplicateExists)
        {
            Response = new UpdateNounResponse { Success = false, Error = $"Another noun with '{req.German}' already exists" };
            return;
        }

        noun.German = req.German.Trim();
        noun.Article = req.Article?.Trim().ToLower() ?? string.Empty;
        noun.TranslationEs = req.TranslationEs.Trim();
        noun.Level = req.Level.Trim();
        noun.Plural = req.Plural?.Trim();
        noun.Notes = req.Notes?.Trim();

        await Db.SaveChangesAsync(ct);

        Response = new UpdateNounResponse { Success = true };
    }
}
