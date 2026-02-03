namespace MemoryApp.Data.Models;

public class VerbForm
{
    public int Id { get; set; }
    public int VerbId { get; set; }
    public string Tense { get; set; } = string.Empty; // present, perfekt, praeteritum
    public string Person { get; set; } = string.Empty; // ich, du, er, wir, ihr, sie
    public string Form { get; set; } = string.Empty;

    public Verb Verb { get; set; } = null!;
}
