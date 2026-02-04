using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Services;

namespace MemoryApp.Endpoints.Matching;

public class GetMatchingSession : Endpoint<GetMatchingSessionRequest, GetMatchingSessionResponse>
{
    public MatchingSessionService MatchingSessionService { get; set; } = null!;

    public override void Configure()
    {
        Get("/api/v1/modes/matching/session");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetMatchingSessionRequest req, CancellationToken ct)
    {
        Response = await MatchingSessionService.GetSessionAsync(req.Level, req.Count, ct);
    }
}
