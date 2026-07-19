using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.Data;
using TimeTracker.Api.DTOs;
using TimeTracker.Api.Models;

namespace TimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ClientDto>>> GetAll()
    {
        var clients = await db.Clients
            .Select(c => new ClientDto(c.Id, c.Name, c.ContactEmail, c.Note))
            .ToListAsync();

        return Ok(clients);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClientDto>> GetById(int id)
    {
        var client = await db.Clients.FindAsync(id);
        if (client is null) return NotFound();

        return Ok(new ClientDto(client.Id, client.Name, client.ContactEmail, client.Note));
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create(CreateClientDto dto)
    {
        var client = new Client
        {
            Name = dto.Name,
            ContactEmail = dto.ContactEmail,
            Note = dto.Note
        };

        db.Clients.Add(client);
        await db.SaveChangesAsync();

        var result = new ClientDto(client.Id, client.Name, client.ContactEmail, client.Note);
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateClientDto dto)
    {
        var client = await db.Clients.FindAsync(id);
        if (client is null) return NotFound();

        client.Name = dto.Name;
        client.ContactEmail = dto.ContactEmail;
        client.Note = dto.Note;

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var client = await db.Clients.FindAsync(id);
        if (client is null) return NotFound();

        db.Clients.Remove(client);
        await db.SaveChangesAsync();
        return NoContent();
    }
}