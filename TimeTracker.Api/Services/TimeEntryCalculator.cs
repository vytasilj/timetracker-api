namespace TimeTracker.Api.Services;

public static class TimeEntryCalculator
{
    private static readonly TimeSpan LunchBreakDuration = TimeSpan.FromMinutes(30);

    public static decimal CalculateHours(decimal? hoursOverride, TimeOnly? startTime, TimeOnly? endTime, bool deductLunchBreak)
    {
        if (hoursOverride.HasValue)
        {
            return hoursOverride.Value;
        }

        if (!startTime.HasValue || !endTime.HasValue)
        {
            // This should never happen if DTO validation ran correctly first.
            // It's a safety net, not the primary validation path.
            throw new InvalidOperationException("Cannot calculate hours without either an explicit value or both StartTime and EndTime.");
        }

        var duration = endTime.Value.ToTimeSpan() - startTime.Value.ToTimeSpan();
        if (deductLunchBreak)
        {
            duration -= LunchBreakDuration;
        }

        return (decimal)duration.TotalHours;
    }
}