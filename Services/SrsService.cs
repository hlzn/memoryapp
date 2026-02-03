namespace MemoryApp.Services;

public enum ReviewResult
{
    Know,
    Doubt,
    DontKnow
}

public class SrsService
{
    public (int masteryDelta, TimeSpan nextInterval) CalculateReview(
        int currentMastery,
        TimeSpan currentInterval,
        ReviewResult result)
    {
        int masteryDelta;
        TimeSpan nextInterval;

        switch (result)
        {
            case ReviewResult.Know:
                masteryDelta = 10;
                // Double the interval, max 90 days
                nextInterval = currentInterval == TimeSpan.Zero
                    ? TimeSpan.FromDays(1)
                    : TimeSpan.FromTicks(Math.Min(currentInterval.Ticks * 2, TimeSpan.FromDays(90).Ticks));
                break;

            case ReviewResult.Doubt:
                masteryDelta = 4;
                // Increase interval by 30%
                nextInterval = currentInterval == TimeSpan.Zero
                    ? TimeSpan.FromHours(12)
                    : TimeSpan.FromTicks((long)(currentInterval.Ticks * 1.3));
                break;

            case ReviewResult.DontKnow:
                masteryDelta = -8;
                // Reset to 1 day (or 10 minutes for same session)
                nextInterval = TimeSpan.FromDays(1);
                break;

            default:
                masteryDelta = 0;
                nextInterval = TimeSpan.FromDays(1);
                break;
        }

        return (masteryDelta, nextInterval);
    }

    public int ClampMastery(int mastery)
    {
        return Math.Clamp(mastery, 0, 100);
    }
}
