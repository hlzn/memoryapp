using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using MemoryApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Settings;

public class UpdateAppSettings : Endpoint<UpdateAppSettingsRequest, UpdateAppSettingsResponse>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Put("/api/v1/settings");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateAppSettingsRequest req, CancellationToken ct)
    {
        var settings = await Db.AppSettings.FirstOrDefaultAsync(ct);

        if (settings == null)
        {
            settings = new AppSettings { Id = 1 };
            Db.AppSettings.Add(settings);
        }

        settings.AiFeedbackEnabled = req.AiFeedbackEnabled;
        settings.AiEndpoint = req.AiEndpoint?.Trim();
        settings.AiModel = req.AiModel?.Trim();
        settings.AiApiKey = req.AiApiKey?.Trim();
        settings.AiTimeoutSeconds = req.AiTimeoutSeconds > 0 ? req.AiTimeoutSeconds : 30;
        settings.AiMaxTokens = req.AiMaxTokens > 0 ? req.AiMaxTokens : 150;
        settings.AiSystemPrompt = req.AiSystemPrompt?.Trim();

        await Db.SaveChangesAsync(ct);

        Response = new UpdateAppSettingsResponse { Success = true };
    }
}
