namespace TimeTracker.Api.Models;

public class Client
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? ContactEmail { get; set; }
    public string? Note { get; set; }

    public List<Project> Projects { get; set; } = [];
}