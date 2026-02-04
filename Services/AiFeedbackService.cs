using System.Net.Http.Json;
using System.Text.Json;
using MemoryApp.Contracts;
using MemoryApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Services;

public class AiFeedbackService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppDbContext _db;

    public AiFeedbackService(IHttpClientFactory httpClientFactory, AppDbContext db)
    {
        _httpClientFactory = httpClientFactory;
        _db = db;
    }

    public async Task<string?> GetFeedbackAsync(
        string correctWord,
        string correctArticle,
        string submittedWord,
        string submittedArticle,
        string spanishTranslation,
        List<MatchingAnswerContextDto>? previousResponses,
        CancellationToken ct = default)
    {
        var settings = await _db.AppSettings.FirstOrDefaultAsync(ct);
        if (settings == null || !settings.AiFeedbackEnabled || string.IsNullOrEmpty(settings.AiEndpoint))
        {
            return null;
        }

        try
        {
            var prompt = BuildPrompt(
                correctWord,
                correctArticle,
                submittedWord,
                submittedArticle,
                spanishTranslation,
                previousResponses,
                settings.AiSystemPrompt);
            var response = await CallAiAsync(
                settings.AiEndpoint,
                settings.AiModel ?? "llama2",
                settings.AiApiKey,
                prompt,
                settings.AiTimeoutSeconds,
                settings.AiMaxTokens,
                ct);
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AI Feedback error: {ex.Message}");
            return null;
        }
    }

    public async Task<(bool Success, string Message)> TestConnectionAsync(
        string endpoint,
        string model,
        string? apiKey,
        int timeoutSeconds,
        int maxTokens,
        CancellationToken ct = default)
    {
        try
        {
            var response = await CallAiAsync(
                endpoint,
                model,
                apiKey,
                "Responde solo con 'OK' si puedes leer este mensaje.",
                timeoutSeconds,
                maxTokens,
                ct);
            if (!string.IsNullOrEmpty(response))
            {
                return (true, $"Conexión exitosa. Respuesta: {response}");
            }
            return (false, "No se recibió respuesta del modelo");
        }
        catch (HttpRequestException ex)
        {
            return (false, $"Error de conexión: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return (false, "Tiempo de espera agotado");
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}");
        }
    }

    private string BuildPrompt(
        string correctWord,
        string correctArticle,
        string submittedWord,
        string submittedArticle,
        string spanishTranslation,
        List<MatchingAnswerContextDto>? previousResponses,
        string? customSystemPrompt)
    {
        var systemPrompt = customSystemPrompt ?? GetDefaultSystemPrompt();

        var wordCorrect = string.Equals(correctWord, submittedWord, StringComparison.OrdinalIgnoreCase);
        var articleCorrect = string.Equals(correctArticle, submittedArticle, StringComparison.OrdinalIgnoreCase);

        var priorContext = "";
        if (previousResponses is { Count: > 0 })
        {
            var recent = previousResponses
                .TakeLast(5)
                .Select(r =>
                {
                    var submitted = string.IsNullOrWhiteSpace(r.SubmittedArticle)
                        ? r.SubmittedGerman
                        : $"{r.SubmittedArticle} {r.SubmittedGerman}";
                    var correctness = r.WasCorrect ? "CORRECTO" : "INCORRECTO";
                    return $"- '{r.PromptEs}' → '{submitted}' ({correctness})";
                });

            priorContext = "\nRespuestas previas recientes:\n" + string.Join('\n', recent) + "\n";
        }

        var correctPhrase = string.IsNullOrWhiteSpace(correctArticle)
            ? correctWord
            : $"{correctArticle} {correctWord}";
        var submittedPhrase = string.IsNullOrWhiteSpace(submittedArticle)
            ? submittedWord
            : $"{submittedArticle} {submittedWord}";

        var context = $@"
El estudiante está aprendiendo alemán. Se le mostró la palabra en español '{spanishTranslation}'.
La respuesta correcta es: {correctPhrase}
El estudiante escribió: {submittedPhrase}

Resultado:
- Artículo: {(articleCorrect ? "CORRECTO" : "INCORRECTO")}
- Palabra: {(wordCorrect ? "CORRECTA" : "INCORRECTA")}

{priorContext}
{systemPrompt}";

        return context;
    }

    private string GetDefaultSystemPrompt()
    {
        return @"Da un comentario breve, amigable y motivador (máximo 2 oraciones) para ayudar al estudiante a recordar mejor.
Si el artículo está mal, menciona una regla o truco para recordar el género.
Si la palabra está mal pero similar, menciona qué tan cerca estuvo.
Usa español para el comentario.
No repitas la respuesta correcta, solo da el tip o comentario motivador.";
    }

    private async Task<string?> CallAiAsync(
        string endpoint,
        string model,
        string? apiKey,
        string prompt,
        int timeoutSeconds,
        int maxTokens,
        CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

        // Detect endpoint type and format request accordingly
        if (endpoint.Contains("openai.com") || endpoint.Contains("/v1/chat/completions"))
        {
            return await CallOpenAiCompatibleAsync(client, endpoint, model, apiKey, prompt, maxTokens, ct);
        }
        else if (endpoint.Contains("11434") || endpoint.Contains("ollama"))
        {
            return await CallOllamaAsync(client, endpoint, model, prompt, maxTokens, ct);
        }
        else
        {
            // Try OpenAI-compatible format by default
            return await CallOpenAiCompatibleAsync(client, endpoint, model, apiKey, prompt, maxTokens, ct);
        }
    }

    private async Task<string?> CallOllamaAsync(
        HttpClient client,
        string endpoint,
        string model,
        string prompt,
        int maxTokens,
        CancellationToken ct)
    {
        var baseUrl = endpoint.TrimEnd('/');
        var url = $"{baseUrl}/api/generate";

        object request;
        if (maxTokens > 0)
        {
            request = new
            {
                model = model,
                prompt = prompt,
                stream = false,
                options = new { num_predict = maxTokens }
            };
        }
        else
        {
            request = new
            {
                model = model,
                prompt = prompt,
                stream = false
            };
        }

        var response = await client.PostAsJsonAsync(url, request, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(ct);
        return result?.Response?.Trim();
    }

    private async Task<string?> CallOpenAiCompatibleAsync(
        HttpClient client,
        string endpoint,
        string model,
        string? apiKey,
        string prompt,
        int maxTokens,
        CancellationToken ct)
    {
        var baseUrl = endpoint.TrimEnd('/');
        var url = baseUrl.EndsWith("/chat/completions") ? baseUrl : $"{baseUrl}/v1/chat/completions";

        if (!string.IsNullOrEmpty(apiKey))
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        }

        var request = new
        {
            model = model,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            max_tokens = maxTokens > 0 ? maxTokens : 150,
            temperature = 0.7
        };

        var response = await client.PostAsJsonAsync(url, request, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OpenAiResponse>(ct);
        return result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();
    }

    private class OllamaResponse
    {
        public string? Response { get; set; }
    }

    private class OpenAiResponse
    {
        public List<OpenAiChoice>? Choices { get; set; }
    }

    private class OpenAiChoice
    {
        public OpenAiMessage? Message { get; set; }
    }

    private class OpenAiMessage
    {
        public string? Content { get; set; }
    }
}
