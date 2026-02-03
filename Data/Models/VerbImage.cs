namespace MemoryApp.Data.Models;

public class VerbImage
{
    public int VerbId { get; set; }
    public int ImageId { get; set; }

    public Verb Verb { get; set; } = null!;
    public Image Image { get; set; } = null!;
}
