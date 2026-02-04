using MemoryApp.Contracts;
using MemoryApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Services;

public class MatchingSessionService
{
    private const int DefaultCount = 10;
    private const int MaxCount = 50;

    private readonly AppDbContext _db;
    private readonly UserService _userService;

    public MatchingSessionService(AppDbContext db, UserService userService)
    {
        _db = db;
        _userService = userService;
    }

    public async Task<GetMatchingSessionResponse> GetSessionAsync(string? level, int count, CancellationToken ct = default)
    {
        var user = await _userService.GetOrCreateDefaultUserAsync();
        var sessionId = Guid.NewGuid();

        var query = _db.Nouns.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(level))
        {
            query = query.Where(n => n.Level == level);
        }

        count = NormalizeCount(count);

        // Step 1: Get user progress for all nouns
        var userProgress = await _db.UserNounProgress
            .AsNoTracking()
            .Where(p => p.UserId == user.Id)
            .ToDictionaryAsync(p => p.NounId, p => p.Mastery, ct);

        // Step 2: Get nouns from query
        var nouns = await query.ToListAsync(ct);

        // Step 3: Combine nouns with their progress
        var nounsWithProgress = nouns.Select(noun => new
        {
            Noun = noun,
            Progress = userProgress.TryGetValue(noun.Id, out var mastery) 
            ? new { Mastery = mastery } 
            : null
        }).ToList();

        // Step 4: Sort by mastery (nulls first for new words), then randomly
        var random = new Random();
        nounsWithProgress = nounsWithProgress
            .OrderBy(x => x.Progress?.Mastery ?? 0)
            .ThenBy(x => random.Next())
            .Take(count)
            .ToList();

        var cards = nounsWithProgress.Select(x => new MatchingCardDto
        {
            NounId = x.Noun.Id,
            TranslationEs = x.Noun.TranslationEs,
            Level = x.Noun.Level,
            CurrentMastery = x.Progress?.Mastery ?? 0
        }).ToList();

        return new GetMatchingSessionResponse
        {
            SessionId = sessionId,
            Cards = cards
        };
    }

    private static int NormalizeCount(int count)
    {
        if (count <= 0)
        {
            return DefaultCount;
        }

        return Math.Min(count, MaxCount);
    }
}
