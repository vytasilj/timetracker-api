namespace TimeTracker.Api.Services;

public static class TimeEntryCalculator
{
    private static readonly TimeSpan LunchBreakDuration = TimeSpan.FromMinutes(30);

    public static decimal? CalculateHours(decimal? hoursOverride, TimeOnly? startTime, TimeOnly? endTime, bool deductLunchBreak)
    {
        if (hoursOverride.HasValue)
        {
            return hoursOverride.Value;
        }

        if (!startTime.HasValue || !endTime.HasValue)
        {
            // Entry has only a start time (or nothing at all) — it's still "open"/in progress.
            // Hours will be calculated later once an end time is provided via update.
            return null;
        }

        var duration = endTime.Value.ToTimeSpan() - startTime.Value.ToTimeSpan();
        if (deductLunchBreak)
        {
            duration -= LunchBreakDuration;
        }

        return (decimal)duration.TotalHours;
    }
}