namespace MemoryApp.Data.Models;

public class Verb
{
    public int Id { get; set; }
    public string Infinitive { get; set; } = string.Empty;
    public string TranslationEs { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty; // A1, A2, B1, B2
    public bool IsSeparable { get; set; }
    public string? Prefix { get; set; }
    public bool IsIrregular { get; set; }
    public string Auxiliary { get; set; } = string.Empty; // haben or sein
    public string PartizipII { get; set; } = string.Empty;
    public string? PraeteritumIch { get; set; }
    public string? Notes { get; set; }

    public ICollection<VerbForm> Forms { get; set; } = new List<VerbForm>();
    public ICollection<VerbImage> VerbImages { get; set; } = new List<VerbImage>();
    public ICollection<Sentence> Sentences { get; set; } = new List<Sentence>();
    public ICollection<UserVerbProgress> UserProgress { get; set; } = new List<UserVerbProgress>();
}
