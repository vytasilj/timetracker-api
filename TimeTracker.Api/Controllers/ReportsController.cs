using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.Data;
using TimeTracker.Api.DTOs;

namespace TimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController(AppDbContext db) : ControllerBase
{
    [HttpGet("monthly-summary")]
    public async Task<ActionResult<MonthlySummaryDto>> GetMonthlySummary([FromQuery] int year, [FromQuery] int month)
    {
        if (year < 2000 || year > 2100)
        {
            return BadRequest("Year must be between 2000 and 2100.");
        }

        if (month < 1 || month > 12)
        {
            return BadRequest("Month must be between 1 and 12.");
        }

        var startDate = new DateOnly(year, month, 1);
        var endDate = startDate.AddMonths(1);

        var entries = await db.TimeEntries
            .Include(t => t.Project)
            .ThenInclude(p => p!.Client)
            .Where(t => t.Date >= startDate && t.Date < endDate)
            .ToListAsync();

        var clientSummaries = entries
            .GroupBy(t => t.Project!.Client!)
            .Select(clientGroup =>
            {
                var projectSummaries = clientGroup
                    .GroupBy(t => t.Project!)
                    .Select(projectGroup => new ProjectSummaryDto(
                        projectGroup.Key.Id,
                        projectGroup.Key.Name,
                        projectGroup.Sum(t => t.Hours),
                        Math.Round(projectGroup.Sum(t => t.Hours * t.EffectiveHourlyRate), 2)
                    ))
                    .OrderBy(p => p.ProjectName)
                    .ToList();

                return new ClientSummaryDto(
                    clientGroup.Key.Id,
                    clientGroup.Key.Name,
                    projectSummaries.Sum(p => p.TotalHours),
                    projectSummaries.Sum(p => p.TotalEarnings),
                    projectSummaries
                );
            })
            .OrderBy(c => c.ClientName)
            .ToList();

        var summary = new MonthlySummaryDto(
            year,
            month,
            clientSummaries.Sum(c => c.TotalHours),
            clientSummaries.Sum(c => c.TotalEarnings),
            clientSummaries
        );

        return Ok(summary);
    }
}