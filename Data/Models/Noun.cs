namespace MemoryApp.Data.Models;

public class Noun
{
    public int Id { get; set; }
    public string German { get; set; } = string.Empty;
    public string Article { get; set; } = string.Empty; // der, die, das
    public string TranslationEs { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty; // A1, A2, B1, B2
    public string? Plural { get; set; }
    public string? Notes { get; set; }

    public ICollection<UserNounProgress> UserProgress { get; set; } = new List<UserNounProgress>();
}
