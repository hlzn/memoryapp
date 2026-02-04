namespace MemoryApp.Data.Models;

public class UserNounProgress
{
    public Guid UserId { get; set; }
    public int NounId { get; set; }
    public int Mastery { get; set; } = 0; // 0-100
    public DateTime NextReviewAt { get; set; } = DateTime.UtcNow;
    public int Streak { get; set; } = 0;
    public string LastResult { get; set; } = string.Empty; // correct, partial, wrong

    public User User { get; set; } = null!;
    public Noun Noun { get; set; } = null!;
}
