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
    public decimal DefaultHourlyRate { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;

    public List<TimeEntry> TimeEntries { get; set; } = [];
}