namespace MemoryApp.Contracts;

public record VerbDto
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

public record GetVerbsRequest
{
    public string? Level { get; init; }
    public int Take { get; init; } = 50;
    public int Skip { get; init; } = 0;
}

public record GetVerbsResponse
{
    public List<VerbDto> Verbs { get; init; } = new();
    public int Total { get; init; }
}
