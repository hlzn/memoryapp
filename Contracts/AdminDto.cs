namespace MemoryApp.Contracts;

public record CreateVerbRequest
{
    public string Infinitive { get; init; } = string.Empty;
    public string TranslationEs { get; init; } = string.Empty;
    public string Level { get; init; } = string.Empty;
    public bool IsSeparable { get; init; }
    public string? Prefix { get; init; }
    public bool IsIrregular { get; init; }
    public string Auxiliary { get; init; } = string.Empty;
    public string PartizipII { get; init; } = string.Empty;
    public string? PraeteritumIch { get; init; }
    public string? Notes { get; init; }
}

public record CreateVerbResponse
{
    public bool Success { get; init; }
    public int? VerbId { get; init; }
    public string? Error { get; init; }
}

public record UpdateVerbRequest
{
    public int Id { get; init; }
    public string Infinitive { get; init; } = string.Empty;
    public string TranslationEs { get; init; } = string.Empty;
    public string Level { get; init; } = string.Empty;
    public bool IsSeparable { get; init; }
    public string? Prefix { get; init; }
    public bool IsIrregular { get; init; }
    public string Auxiliary { get; init; } = string.Empty;
    public string PartizipII { get; init; } = string.Empty;
    public string? PraeteritumIch { get; init; }
    public string? Notes { get; init; }
}

public record UpdateVerbResponse
{
    public bool Success { get; init; }
    public string? Error { get; init; }
}

public record DeleteVerbRequest
{
    public int Id { get; init; }
}

public record DeleteVerbResponse
{
    public bool Success { get; init; }
    public string? Error { get; init; }
}

public record ImportCsvRequest
{
    public string CsvContent { get; init; } = string.Empty;
}

public record ImportCsvResponse
{
    public bool Success { get; init; }
    public int ImportedCount { get; init; }
    public int SkippedCount { get; init; }
    public List<string> Errors { get; init; } = new();
}
