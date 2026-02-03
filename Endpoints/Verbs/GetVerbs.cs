using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Verbs;

public class GetVerbs : Endpoint<GetVerbsRequest, GetVerbsResponse>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Get("/api/v1/verbs");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetVerbsRequest req, CancellationToken ct)
    {
        var query = Db.Verbs.AsQueryable();

        if (!string.IsNullOrEmpty(req.Level))
        {
            query = query.Where(v => v.Level == req.Level);
        }

        var total = await query.CountAsync(ct);

        var verbs = await query
            .OrderBy(v => v.Infinitive)
            .Skip(req.Skip)
            .Take(req.Take)
            .Select(v => new VerbDto
            {
                Id = v.Id,
                Infinitive = v.Infinitive,
                TranslationEs = v.TranslationEs,
                Level = v.Level,
                IsSeparable = v.IsSeparable,
                Prefix = v.Prefix,
                IsIrregular = v.IsIrregular,
                Auxiliary = v.Auxiliary,
                PartizipII = v.PartizipII,
                PraeteritumIch = v.PraeteritumIch,
                Notes = v.Notes
            })
            .ToListAsync(ct);

        Response = new GetVerbsResponse
        {
            Verbs = verbs,
            Total = total
        };
    }
}
