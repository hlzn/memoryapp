namespace MemoryApp.Data.Models;

public class User
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public UserSettings? Settings { get; set; }
    public ICollection<UserVerbProgress> VerbProgress { get; set; } = new List<UserVerbProgress>();
    public ICollection<UserNounProgress> NounProgress { get; set; } = new List<UserNounProgress>();
    public ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
}
