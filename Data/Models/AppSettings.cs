namespace MemoryApp.Data.Models;

public class AppSettings
{
    public int Id { get; set; } = 1; // Single row table

    // AI Feedback Settings
    public bool AiFeedbackEnabled { get; set; } = false;
    public string? AiEndpoint { get; set; } // e.g., "http://localhost:11434" for Ollama
    public string? AiModel { get; set; } // e.g., "llama2", "mistral", "gpt-4"
    public string? AiApiKey { get; set; } // Optional API key for services that require it
    public int AiTimeoutSeconds { get; set; } = 30;
    public int AiMaxTokens { get; set; } = 150;
    public string? AiSystemPrompt { get; set; } // Custom system prompt for feedback
}
