using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.Data;
using TimeTracker.Api.DTOs;
using TimeTracker.Api.Models;
using TimeTracker.Api.Services;

namespace TimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TimeEntriesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TimeEntryDto>>> GetAll()
    {
        var entries = await db.TimeEntries
            .Include(t => t.Project)
            .ThenInclude(p => p!.Rates)
            .OrderByDescending(t => t.Date)
            .ToListAsync();

        return Ok(entries.Select(ToDto).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TimeEntryDto>> GetById(int id)
    {
        var entry = await db.TimeEntries
            .Include(t => t.Project)
            .ThenInclude(p => p!.Rates)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (entry is null) return NotFound();

        return Ok(ToDto(entry));
    }

    [HttpPost]
    public async Task<ActionResult<TimeEntryDto>> Create(CreateTimeEntryDto dto)
    {
        var project = await db.Projects.FindAsync(dto.ProjectId);
        if (project is null)
        {
            return BadRequest($"Project with id {dto.ProjectId} does not exist.");
        }

        var hours = TimeEntryCalculator.CalculateHours(dto.Hours, dto.StartTime, dto.EndTime, dto.DeductLunchBreak);

        var entry = new TimeEntry
        {
            ProjectId = dto.ProjectId,
            Date = dto.Date,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            DeductLunchBreak = dto.DeductLunchBreak,
            Hours = hours.HasValue ? Math.Round(hours.Value, 2, MidpointRounding.AwayFromZero) : null,
            HourlyRateOverride = dto.HourlyRateOverride,
            Description = dto.Description
        };

        db.TimeEntries.Add(entry);
        await db.SaveChangesAsync();

        entry.Project = project;
        return CreatedAtAction(nameof(GetById), new { id = entry.Id }, ToDto(entry));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTimeEntryDto dto)
    {
        var entry = await db.TimeEntries.Include(t => t.Project)
            .ThenInclude(p => p!.Rates)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (entry is null) return NotFound();

        var hours = TimeEntryCalculator.CalculateHours(dto.Hours, dto.StartTime, dto.EndTime, dto.DeductLunchBreak);

        entry.Date = dto.Date;
        entry.StartTime = dto.StartTime;
        entry.EndTime = dto.EndTime;
        entry.DeductLunchBreak = dto.DeductLunchBreak;
        entry.Hours = hours.HasValue ? Math.Round(hours.Value, 2, MidpointRounding.AwayFromZero) : null;
        entry.HourlyRateOverride = dto.HourlyRateOverride;
        entry.Description = dto.Description;
        entry.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entry = await db.TimeEntries.FindAsync(id);
        if (entry is null) return NotFound();

        db.TimeEntries.Remove(entry);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static TimeEntryDto ToDto(TimeEntry entry) => new(
        entry.Id,
        entry.ProjectId,
        entry.Project!.Name,
        entry.Date,
        entry.StartTime,
        entry.EndTime,
        entry.DeductLunchBreak,
        entry.Hours,
        entry.HourlyRateOverride,
        entry.EffectiveHourlyRate,
        entry.Description
    );
}