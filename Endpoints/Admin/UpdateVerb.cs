using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Admin;

public class UpdateVerb : Endpoint<UpdateVerbRequest, UpdateVerbResponse>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Put("/api/v1/admin/verbs");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateVerbRequest req, CancellationToken ct)
    {
        var verb = await Db.Verbs.FindAsync(new object[] { req.Id }, ct);
        if (verb == null)
        {
            Response = new UpdateVerbResponse { Success = false, Error = "Verb not found" };
            return;
        }

        var duplicateExists = await Db.Verbs
            .AnyAsync(v => v.Infinitive == req.Infinitive && v.Id != req.Id, ct);
        if (duplicateExists)
        {
            Response = new UpdateVerbResponse { Success = false, Error = $"Another verb with infinitive '{req.Infinitive}' already exists" };
            return;
        }

        verb.Infinitive = req.Infinitive.Trim();
        verb.TranslationEs = req.TranslationEs.Trim();
        verb.Level = req.Level.Trim();
        verb.IsSeparable = req.IsSeparable;
        verb.Prefix = req.Prefix?.Trim();
        verb.IsIrregular = req.IsIrregular;
        verb.Auxiliary = req.Auxiliary.Trim();
        verb.PartizipII = req.PartizipII.Trim();
        verb.PraeteritumIch = req.PraeteritumIch?.Trim();
        verb.Notes = req.Notes?.Trim();

        await Db.SaveChangesAsync(ct);

        Response = new UpdateVerbResponse { Success = true };
    }
}
