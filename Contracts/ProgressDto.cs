namespace MemoryApp.Contracts;

public record ProgressOverviewResponse
{
    public string Level { get; init; } = string.Empty;
    public int TotalVerbs { get; init; }
    public int MasteredVerbs { get; init; }
    public int LearningVerbs { get; init; }
    public int NewVerbs { get; init; }
    public double AverageMastery { get; init; }
    public int TotalAttempts { get; init; }
    public int CorrectAttempts { get; init; }
    public double AccuracyRate { get; init; }
}

public record VerbProgressDto
{
    public int VerbId { get; init; }
    public string Infinitive { get; init; } = string.Empty;
    public string TranslationEs { get; init; } = string.Empty;
    public int Mastery { get; init; }
    public DateTime NextReviewAt { get; init; }
    public int Streak { get; init; }
    public string LastResult { get; init; } = string.Empty;
}

public record GetProgressRequest
{
    public string? Level { get; init; }
}
