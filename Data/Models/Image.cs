namespace MemoryApp.Data.Models;

public class Image
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;
    public string PathOrData { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty; // line, flat, etc.

    public ICollection<VerbImage> VerbImages { get; set; } = new List<VerbImage>();
}
