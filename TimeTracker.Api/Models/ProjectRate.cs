namespace TimeTracker.Api.Models;

public class ProjectRate
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }

    public decimal HourlyRate { get; set; }
    public DateOnly EffectiveFrom { get; set; }
}