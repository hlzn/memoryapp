using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;

namespace MemoryApp.Endpoints.Nouns;

public class DeleteNoun : Endpoint<DeleteNounRequest, DeleteNounResponse>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Delete("/api/v1/nouns/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteNounRequest req, CancellationToken ct)
    {
        var noun = await Db.Nouns.FindAsync(new object[] { req.Id }, ct);
        if (noun == null)
        {
            Response = new DeleteNounResponse { Success = false, Error = "Noun not found" };
            return;
        }

        Db.Nouns.Remove(noun);
        await Db.SaveChangesAsync(ct);

        Response = new DeleteNounResponse { Success = true };
    }
}
