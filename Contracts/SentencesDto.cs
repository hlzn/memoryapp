namespace MemoryApp.Contracts;

public record SentenceQuestionDto
{
    public int SentenceId { get; init; }
    public string SentenceTemplate { get; init; } = string.Empty;
    public string? TranslationEs { get; init; }
    public string? Hint { get; init; }
    public string Type { get; init; } = string.Empty;
    public List<string>? WordBank { get; init; }
}

public record GetSentencesSessionRequest
{
    public string Level { get; init; } = "A1";
    public string Difficulty { get; init; } = "easy"; // easy, medium, hard
    public int Count { get; init; } = 10;
}

public record GetSentencesSessionResponse
{
    public Guid SessionId { get; init; }
    public List<SentenceQuestionDto> Questions { get; init; } = new();
}

public record CheckSentenceAnswerRequest
{
    public Guid SessionId { get; init; }
    public int SentenceId { get; init; }
    public string Answer { get; init; } = string.Empty;
}

public record CheckSentenceAnswerResponse
{
    public bool IsCorrect { get; init; }
    public string CorrectAnswer { get; init; } = string.Empty;
    public string? Explanation { get; init; }
    public string Message { get; init; } = string.Empty;
}
