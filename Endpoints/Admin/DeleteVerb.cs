using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;

namespace MemoryApp.Endpoints.Admin;

public class DeleteVerb : Endpoint<DeleteVerbRequest, DeleteVerbResponse>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Delete("/api/v1/admin/verbs/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteVerbRequest req, CancellationToken ct)
    {
        var verb = await Db.Verbs.FindAsync(new object[] { req.Id }, ct);
        if (verb == null)
        {
            Response = new DeleteVerbResponse { Success = false, Error = "Verb not found" };
            return;
        }

        Db.Verbs.Remove(verb);
        await Db.SaveChangesAsync(ct);

        Response = new DeleteVerbResponse { Success = true };
    }
}
