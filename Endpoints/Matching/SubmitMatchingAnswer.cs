using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Services;

namespace MemoryApp.Endpoints.Matching;

public class SubmitMatchingAnswer : Endpoint<SubmitMatchingAnswerRequest, SubmitMatchingAnswerResponse>
{
    public MatchingAnswerService MatchingAnswerService { get; set; } = null!;

    public override void Configure()
    {
        Post("/api/v1/modes/matching/answer");
        AllowAnonymous();
    }

    public override async Task HandleAsync(SubmitMatchingAnswerRequest req, CancellationToken ct)
    {
        Response = await MatchingAnswerService.SubmitAsync(req, ct);
    }
}
