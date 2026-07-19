using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.Data;
using TimeTracker.Api.DTOs;
using TimeTracker.Api.Models;

namespace TimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll()
    {
        var projects = await db.Projects
            .Include(p => p.Client)
            .Select(p => new ProjectDto(p.Id, p.ClientId, p.Client!.Name, p.Name, p.DefaultHourlyRate, p.Status))
            .ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id)
    {
        var project = await db.Projects
            .Include(p => p.Client)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project is null) return NotFound();

        return Ok(new ProjectDto(project.Id, project.ClientId, project.Client!.Name, project.Name, project.DefaultHourlyRate, project.Status));
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(CreateProjectDto dto)
    {
        var clientExists = await db.Clients.AnyAsync(c => c.Id == dto.ClientId);
        if (!clientExists)
        {
            return BadRequest($"Client with id {dto.ClientId} does not exist.");
        }

        var project = new Project
        {
            ClientId = dto.ClientId,
            Name = dto.Name,
            DefaultHourlyRate = dto.DefaultHourlyRate,
            Status = ProjectStatus.Active
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var client = await db.Clients.FindAsync(dto.ClientId);
        var result = new ProjectDto(project.Id, project.ClientId, client!.Name, project.Name, project.DefaultHourlyRate, project.Status);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProjectDto dto)
    {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return NotFound();

        project.Name = dto.Name;
        project.DefaultHourlyRate = dto.DefaultHourlyRate;
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
}