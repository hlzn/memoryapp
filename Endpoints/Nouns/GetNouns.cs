using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Nouns;

public class GetNouns : Endpoint<GetNounsRequest, GetNounsResponse>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Get("/api/v1/nouns");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetNounsRequest req, CancellationToken ct)
    {
        var query = Db.Nouns.AsQueryable();

        if (!string.IsNullOrEmpty(req.Level))
        {
            query = query.Where(n => n.Level == req.Level);
        }

        var total = await query.CountAsync(ct);

        var nouns = await query
            .OrderBy(n => n.German)
            .Skip(req.Skip)
            .Take(req.Take)
            .Select(n => new NounDto
            {
                Id = n.Id,
                German = n.German,
                Article = n.Article,
                TranslationEs = n.TranslationEs,
                Level = n.Level,
                Plural = n.Plural,
                Notes = n.Notes
            })
            .ToListAsync(ct);

        Response = new GetNounsResponse
        {
            Nouns = nouns,
            Total = total
        };
    }
}
