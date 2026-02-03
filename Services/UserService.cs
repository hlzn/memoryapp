using MemoryApp.Data;
using MemoryApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Services;

public class UserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User> GetOrCreateDefaultUserAsync()
    {
        // For MVP, use a single default user
        const string defaultUserIdString = "00000000-0000-0000-0000-000000000001";
        var defaultUserId = Guid.Parse(defaultUserIdString);

        var user = await _db.Users
            .Include(u => u.Settings)
            .FirstOrDefaultAsync(u => u.Id == defaultUserId);

        if (user == null)
        {
            user = new User
            {
                Id = defaultUserId,
                DisplayName = "Estudiante",
                CreatedAt = DateTime.UtcNow,
                Settings = new UserSettings
                {
                    UserId = defaultUserId,
                    PreferredLevel = "A1",
                    DailyGoal = 20,
                    UiTheme = "light"
                }
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        return user;
    }
}
