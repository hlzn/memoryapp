using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Services;

namespace MemoryApp.Endpoints.Settings;

public class TestAiConnection : Endpoint<TestAiConnectionRequest, TestAiConnectionResponse>
{
    public AiFeedbackService AiFeedbackService { get; set; } = null!;

    public override void Configure()
    {
        Post("/api/v1/settings/test-ai");
        AllowAnonymous();
    }

    public override async Task HandleAsync(TestAiConnectionRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Endpoint))
        {
            Response = new TestAiConnectionResponse { Success = false, Message = "El endpoint es requerido" };
            return;
        }

        if (string.IsNullOrWhiteSpace(req.Model))
        {
            Response = new TestAiConnectionResponse { Success = false, Message = "El modelo es requerido" };
            return;
        }

        var (success, message) = await AiFeedbackService.TestConnectionAsync(
            req.Endpoint,
            req.Model,
            req.ApiKey,
            req.TimeoutSeconds,
            req.MaxTokens,
            ct);

        Response = new TestAiConnectionResponse { Success = success, Message = message };
    }
}
