namespace TimeTracker.Api.Models;

public class TimeEntry
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }

    public DateOnly Date { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public bool DeductLunchBreak { get; set; } = false;

    public decimal Hours { get; set; }
    public decimal? HourlyRateOverride { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public decimal EffectiveHourlyRate => HourlyRateOverride ?? Project?.DefaultHourlyRate ?? 0;
}