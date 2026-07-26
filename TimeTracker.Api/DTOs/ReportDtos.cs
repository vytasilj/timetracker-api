namespace TimeTracker.Api.DTOs;

public record ProjectSummaryDto(
    int ProjectId,
    string ProjectName,
    decimal TotalHours,
    decimal TotalEarnings
);

public record ClientSummaryDto(
    int ClientId,
    string ClientName,
    decimal TotalHours,
    decimal TotalEarnings,
    List<ProjectSummaryDto> Projects
);

public record MonthlySummaryDto(
    int Year,
    int Month,
    decimal TotalHours,
    decimal TotalEarnings,
    List<ClientSummaryDto> Clients
);