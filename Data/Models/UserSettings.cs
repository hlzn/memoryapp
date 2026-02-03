namespace MemoryApp.Data.Models;

public class UserSettings
{
    public Guid UserId { get; set; }
    public string PreferredLevel { get; set; } = "A1";
    public int DailyGoal { get; set; } = 20;
    public string UiTheme { get; set; } = "light";

    public User User { get; set; } = null!;
}
