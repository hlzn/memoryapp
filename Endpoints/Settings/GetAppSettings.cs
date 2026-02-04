using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using MemoryApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Settings;

public class GetAppSettings : EndpointWithoutRequest<GetAppSettingsResponse>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Get("/api/v1/settings");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var settings = await Db.AppSettings.FirstOrDefaultAsync(ct);

        if (settings == null)
        {
            // Create default settings if not exists
            settings = new AppSettings
            {
                Id = 1,
                AiFeedbackEnabled = false,
                AiTimeoutSeconds = 30,
                AiMaxTokens = 150
            };
            Db.AppSettings.Add(settings);
            await Db.SaveChangesAsync(ct);
        }

        Response = new GetAppSettingsResponse
        {
            Settings = new AppSettingsDto
            {
                AiFeedbackEnabled = settings.AiFeedbackEnabled,
                AiEndpoint = settings.AiEndpoint,
                AiModel = settings.AiModel,
                AiApiKey = settings.AiApiKey,
                AiTimeoutSeconds = settings.AiTimeoutSeconds,
                AiMaxTokens = settings.AiMaxTokens,
                AiSystemPrompt = settings.AiSystemPrompt
            }
        };
    }
}
