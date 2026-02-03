namespace MemoryApp.Data.Models;

public class UserVerbProgress
{
    public Guid UserId { get; set; }
    public int VerbId { get; set; }
    public int Mastery { get; set; } // 0-100
    public DateTime NextReviewAt { get; set; }
    public int Streak { get; set; }
    public string LastResult { get; set; } = string.Empty; // know, doubt, dontknow

    public User User { get; set; } = null!;
    public Verb Verb { get; set; } = null!;
}
