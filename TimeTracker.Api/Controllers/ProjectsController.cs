using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.Data;
using TimeTracker.Api.DTOs;
using TimeTracker.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace TimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll()
    {
        var projects = await db.Projects
            .Include(p => p.Client)
            .Include(p => p.Rates)
            .ToListAsync();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return Ok(projects.Select(p => ToDto(p, today)).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id)
    {
        var project = await db.Projects
            .Include(p => p.Client)
            .Include(p => p.Rates)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project is null) return NotFound();

        return Ok(ToDto(project, DateOnly.FromDateTime(DateTime.UtcNow)));
    }

    [HttpGet("{id:int}/rates")]
    public async Task<ActionResult<List<ProjectRateDto>>> GetRates(int id)
    {
        var exists = await db.Projects.AnyAsync(p => p.Id == id);
        if (!exists) return NotFound();

        var rates = await db.ProjectRates
            .Where(r => r.ProjectId == id)
            .OrderByDescending(r => r.EffectiveFrom)
            .Select(r => new ProjectRateDto(r.Id, r.HourlyRate, r.EffectiveFrom))
            .ToListAsync();

        return Ok(rates);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(CreateProjectDto dto)
    {
        var client = await db.Clients.FindAsync(dto.ClientId);
        if (client is null)
        {
            return BadRequest($"Client with id {dto.ClientId} does not exist.");
        }

        var project = new Project
        {
            ClientId = dto.ClientId,
            Name = dto.Name,
            Status = ProjectStatus.Active,
            Rates = [new ProjectRate { HourlyRate = dto.InitialHourlyRate, EffectiveFrom = dto.EffectiveFrom }]
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync();

        project.Client = client;
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, ToDto(project, DateOnly.FromDateTime(DateTime.UtcNow)));
    }

    [HttpPost("{id:int}/rates")]
    public async Task<ActionResult<ProjectRateDto>> AddRate(int id, CreateProjectRateDto dto)
    {
        var project = await db.Projects.Include(p => p.Rates).FirstOrDefaultAsync(p => p.Id == id);
        if (project is null) return NotFound();

        if (project.Rates.Any(r => r.EffectiveFrom == dto.EffectiveFrom))
        {
            return BadRequest("A rate with this effective date already exists for this project. Delete it first if you want to replace it.");
        }

        var rate = new ProjectRate { ProjectId = id, HourlyRate = dto.HourlyRate, EffectiveFrom = dto.EffectiveFrom };
        db.ProjectRates.Add(rate);
        await db.SaveChangesAsync();

        return Ok(new ProjectRateDto(rate.Id, rate.HourlyRate, rate.EffectiveFrom));
    }

    [HttpDelete("{id:int}/rates/{rateId:int}")]
    public async Task<IActionResult> DeleteRate(int id, int rateId)
    {
        var project = await db.Projects.Include(p => p.Rates).FirstOrDefaultAsync(p => p.Id == id);
        if (project is null) return NotFound();

        var rate = project.Rates.FirstOrDefault(r => r.Id == rateId);
        if (rate is null) return NotFound();

        if (project.Rates.Count == 1)
        {
            return BadRequest("Cannot delete the last remaining rate for a project. A project must always have at least one rate.");
        }

        db.ProjectRates.Remove(rate);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProjectDto dto)
    {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return NotFound();

        project.Name = dto.Name;
        project.Status = dto.Status;

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return NotFound();

        db.Projects.Remove(project);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static ProjectDto ToDto(Project project, DateOnly asOfDate) => new(
        project.Id,
        project.ClientId,
        project.Client!.Name,
        project.Name,
        project.GetRateForDate(asOfDate) ?? 0,
        project.Status
    );
}