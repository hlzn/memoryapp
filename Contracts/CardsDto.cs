namespace MemoryApp.Contracts;

public record CardDto
{
    public int VerbId { get; init; }
    public string Infinitive { get; init; } = string.Empty;
    public string TranslationEs { get; init; } = string.Empty;
    public string PartizipII { get; init; } = string.Empty;
    public string Auxiliary { get; init; } = string.Empty;
    public bool IsSeparable { get; init; }
    public string? Prefix { get; init; }
    public string? Notes { get; init; }
    public int CurrentMastery { get; init; }
}

public record GetCardsSessionRequest
{
    public string Level { get; init; } = "A1";
    public string Type { get; init; } = "review"; // review, new, failed
    public int Count { get; init; } = 20;
}

public record GetCardsSessionResponse
{
    public Guid SessionId { get; init; }
    public List<CardDto> Cards { get; init; } = new();
    public int TotalInLevel { get; init; }
}

public record RateCardRequest
{
    public Guid SessionId { get; init; }
    public int VerbId { get; init; }
    public string Rating { get; init; } = string.Empty; // know, doubt, dontknow
}

public record RateCardResponse
{
    public bool Success { get; init; }
    public int NewMastery { get; init; }
    public DateTime NextReviewAt { get; init; }
    public string Message { get; init; } = string.Empty;
}
