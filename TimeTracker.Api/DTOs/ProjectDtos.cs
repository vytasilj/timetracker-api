using System.ComponentModel.DataAnnotations;
using TimeTracker.Api.Models;

namespace TimeTracker.Api.DTOs;

public record ProjectDto(
    int Id,
    int ClientId,
    string ClientName,
    string Name,
    decimal DefaultHourlyRate,
    ProjectStatus Status
);

public record CreateProjectDto(
    [Required] int ClientId,
    [Required, MinLength(1)] string Name,
    [Range(0, 100000)] decimal DefaultHourlyRate
);

public record UpdateProjectDto(
    [Required, MinLength(1)] string Name,
    [Range(0, 100000)] decimal DefaultHourlyRate,
    ProjectStatus Status
);