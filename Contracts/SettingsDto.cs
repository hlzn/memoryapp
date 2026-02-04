namespace MemoryApp.Contracts;

public record AppSettingsDto
{
    public bool AiFeedbackEnabled { get; init; }
    public string? AiEndpoint { get; init; }
    public string? AiModel { get; init; }
    public string? AiApiKey { get; init; }
    public int AiTimeoutSeconds { get; init; }
    public int AiMaxTokens { get; init; }
    public string? AiSystemPrompt { get; init; }
}

public record GetAppSettingsResponse
{
    public AppSettingsDto Settings { get; init; } = null!;
}

public record UpdateAppSettingsRequest
{
    public bool AiFeedbackEnabled { get; init; }
    public string? AiEndpoint { get; init; }
    public string? AiModel { get; init; }
    public string? AiApiKey { get; init; }
    public int AiTimeoutSeconds { get; init; }
    public int AiMaxTokens { get; init; }
    public string? AiSystemPrompt { get; init; }
}

public record UpdateAppSettingsResponse
{
    public bool Success { get; init; }
    public string? Error { get; init; }
}

public record TestAiConnectionRequest
{
    public string Endpoint { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public string? ApiKey { get; init; }
    public int TimeoutSeconds { get; init; } = 30;
    public int MaxTokens { get; init; } = 150;
}

public record TestAiConnectionResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}
