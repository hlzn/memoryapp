using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Verbs;

public class GetVerbByIdRequest
{
    public int Id { get; set; }
}

public class GetVerbById : Endpoint<GetVerbByIdRequest, VerbDto>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Get("/api/v1/verbs/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetVerbByIdRequest req, CancellationToken ct)
    {
        var verb = await Db.Verbs
            .Where(v => v.Id == req.Id)
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
            .FirstOrDefaultAsync(ct);

        if (verb is null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        Response = verb;
    }
}
