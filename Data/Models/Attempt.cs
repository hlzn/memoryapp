namespace MemoryApp.Data.Models;

public class Attempt
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string Mode { get; set; } = string.Empty; // cards, images, sentences
    public string ItemType { get; set; } = string.Empty; // verb, sentence
    public int ItemId { get; set; }
    public bool IsCorrect { get; set; }
    public int ScoreDelta { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
