using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using MemoryApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Admin;

public class CreateVerb : Endpoint<CreateVerbRequest, CreateVerbResponse>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Post("/api/v1/admin/verbs");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateVerbRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Infinitive))
        {
            Response = new CreateVerbResponse { Success = false, Error = "Infinitive is required" };
            return;
        }

        var exists = await Db.Verbs.AnyAsync(v => v.Infinitive == req.Infinitive, ct);
        if (exists)
        {
            Response = new CreateVerbResponse { Success = false, Error = $"Verb '{req.Infinitive}' already exists" };
            return;
        }

        var verb = new Verb
        {
            Infinitive = req.Infinitive.Trim(),
            TranslationEs = req.TranslationEs.Trim(),
            Level = req.Level.Trim(),
            IsSeparable = req.IsSeparable,
            Prefix = req.Prefix?.Trim(),
            IsIrregular = req.IsIrregular,
            Auxiliary = req.Auxiliary.Trim(),
            PartizipII = req.PartizipII.Trim(),
            PraeteritumIch = req.PraeteritumIch?.Trim(),
            Notes = req.Notes?.Trim()
        };

        Db.Verbs.Add(verb);
        await Db.SaveChangesAsync(ct);

        Response = new CreateVerbResponse { Success = true, VerbId = verb.Id };
    }
}
