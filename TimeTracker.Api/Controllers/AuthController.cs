using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.Data;
using TimeTracker.Api.DTOs;
using TimeTracker.Api.Services;

namespace TimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db, JwtTokenGenerator tokenGenerator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
        {
            // Same error message for "user not found" and "wrong password"
            // to avoid revealing which emails are registered
            return Unauthorized("Invalid email or password.");
        }

        var (token, expiresAt) = tokenGenerator.GenerateToken(user);
        return Ok(new LoginResponseDto(token, expiresAt));
    }
}