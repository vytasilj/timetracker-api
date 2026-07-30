namespace TimeTracker.Api.Models;

public enum ProjectStatus
{
    Active,
    Closed
}

public class Project
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }

    public required string Name { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;

    public List<TimeEntry> TimeEntries { get; set; } = [];
    public List<ProjectRate> Rates { get; set; } = [];

    public decimal? GetRateForDate(DateOnly date)
    {
        if (Rates.Count == 0) return null;

        var applicable = Rates
            .Where(r => r.EffectiveFrom <= date)
            .OrderByDescending(r => r.EffectiveFrom)
            .FirstOrDefault();

        // Fallback: if the entry predates every recorded rate (e.g. rate history
        // was only set up starting later), use the oldest known rate instead of 0.
        return applicable?.HourlyRate ?? Rates.OrderBy(r => r.EffectiveFrom).First().HourlyRate;
    }
}