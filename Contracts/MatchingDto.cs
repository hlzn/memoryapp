namespace MemoryApp.Contracts;

public record GetMatchingSessionRequest
{
    public string? Level { get; init; }
    public int Count { get; init; } = 10;
}

public record GetMatchingSessionResponse
{
    public Guid SessionId { get; init; }
    public List<MatchingCardDto> Cards { get; init; } = new();
}

public record MatchingCardDto
{
    public int NounId { get; init; }
    public string TranslationEs { get; init; } = string.Empty;
    public string Level { get; init; } = string.Empty;
    public int CurrentMastery { get; init; }
}

public record SubmitMatchingAnswerRequest
{
    public Guid SessionId { get; init; }
    public int NounId { get; init; }
    public string SubmittedGerman { get; init; } = string.Empty;
    public string SubmittedArticle { get; init; } = string.Empty;
    public List<MatchingAnswerContextDto> PreviousResponses { get; init; } = new();
}

public record SubmitMatchingAnswerResponse
{
    public bool Success { get; init; }
    public string Result { get; init; } = string.Empty; // correct, partial, wrong
    public string CorrectGerman { get; init; } = string.Empty;
    public string CorrectArticle { get; init; } = string.Empty;
    public int NewMastery { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? AiComment { get; init; } // AI-generated feedback when enabled
}

public record MatchingAnswerContextDto
{
    public string PromptEs { get; init; } = string.Empty;
    public string SubmittedGerman { get; init; } = string.Empty;
    public string SubmittedArticle { get; init; } = string.Empty;
    public bool WasCorrect { get; init; }
}
