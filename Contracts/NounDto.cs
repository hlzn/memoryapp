namespace MemoryApp.Contracts;

public record NounDto
{
    public int Id { get; init; }
    public string German { get; init; } = string.Empty;
    public string Article { get; init; } = string.Empty;
    public string TranslationEs { get; init; } = string.Empty;
    public string Level { get; init; } = string.Empty;
    public string? Plural { get; init; }
    public string? Notes { get; init; }
}

public record GetNounsRequest
{
    public string? Level { get; init; }
    public int Take { get; init; } = 50;
    public int Skip { get; init; } = 0;
}

public record GetNounsResponse
{
    public List<NounDto> Nouns { get; init; } = new();
    public int Total { get; init; }
}

public record CreateNounRequest
{
    public string German { get; init; } = string.Empty;
    public string Article { get; init; } = string.Empty;
    public string TranslationEs { get; init; } = string.Empty;
    public string Level { get; init; } = string.Empty;
    public string? Plural { get; init; }
    public string? Notes { get; init; }
}

public record CreateNounResponse
{
    public bool Success { get; init; }
    public int? NounId { get; init; }
    public string? Error { get; init; }
}

public record UpdateNounRequest
{
    public int Id { get; init; }
    public string German { get; init; } = string.Empty;
    public string Article { get; init; } = string.Empty;
    public string TranslationEs { get; init; } = string.Empty;
    public string Level { get; init; } = string.Empty;
    public string? Plural { get; init; }
    public string? Notes { get; init; }
}

public record UpdateNounResponse
{
    public bool Success { get; init; }
    public string? Error { get; init; }
}

public record DeleteNounRequest
{
    public int Id { get; init; }
}

public record DeleteNounResponse
{
    public bool Success { get; init; }
    public string? Error { get; init; }
}

public record ImportNounsCsvRequest
{
    public string CsvContent { get; init; } = string.Empty;
}

public record ImportNounsCsvResponse
{
    public bool Success { get; init; }
    public int ImportedCount { get; init; }
    public int SkippedCount { get; init; }
    public List<string> Errors { get; init; } = new();
}
