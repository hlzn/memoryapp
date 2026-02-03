namespace MemoryApp.Data.Models;

public class Sentence
{
    public int Id { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // infinitive, present, perfekt, separable, preposition
    public string SentenceTemplate { get; set; } = string.Empty; // "Ich ____ nach Hause."
    public string CorrectAnswer { get; set; } = string.Empty;
    public string? Hint { get; set; }
    public string? TranslationEs { get; set; }
    public int? VerbId { get; set; }

    public Verb? Verb { get; set; }
}
