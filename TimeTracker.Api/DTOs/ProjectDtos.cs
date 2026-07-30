using System.ComponentModel.DataAnnotations;
using TimeTracker.Api.Models;

namespace TimeTracker.Api.DTOs;

public record ProjectRateDto(int Id, decimal HourlyRate, DateOnly EffectiveFrom);

public record ProjectDto(
    int Id,
    int ClientId,
    string ClientName,
    string Name,
    decimal CurrentHourlyRate,
    ProjectStatus Status
);

public record CreateProjectDto(
    [Required] int ClientId,
    [Required, MinLength(1)] string Name,
    [Range(0.01, 100000)] decimal InitialHourlyRate,
    [Required] DateOnly EffectiveFrom
);

public record UpdateProjectDto(
    [Required, MinLength(1)] string Name,
    ProjectStatus Status
);

public record CreateProjectRateDto(
    [Range(0.01, 100000)] decimal HourlyRate,
    [Required] DateOnly EffectiveFrom
);