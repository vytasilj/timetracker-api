using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.Data;
using TimeTracker.Api.DTOs;
using TimeTracker.Api.Services;

namespace TimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db, JwtTokenGenerator tokenGenerator, RefreshTokenService refreshTokenService, IHostEnvironment env) : ControllerBase
{
    private const string CookieName = "refreshToken";

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid email or password.");
        }

        var (token, expiresAt) = tokenGenerator.GenerateToken(user);
        var refreshToken = await refreshTokenService.IssueAsync(user.Id);
        SetRefreshTokenCookie(refreshToken);

        return Ok(new LoginResponseDto(token, expiresAt));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDto>> Refresh()
    {
        var rawToken = Request.Cookies[CookieName];
        if (rawToken is null)
        {
            return Unauthorized("No refresh token provided.");
        }

        var result = await refreshTokenService.ValidateAndRotateAsync(rawToken);
        if (result is null)
        {
            Response.Cookies.Delete(CookieName, new CookieOptions { Path = "/api/auth" });
            return Unauthorized("Refresh token is invalid or expired.");
        }

        var (newRawToken, userId) = result.Value;
        var user = await db.Users.FindAsync(userId);
        if (user is null)
        {
            return Unauthorized();
        }

        var (token, expiresAt) = tokenGenerator.GenerateToken(user);
        SetRefreshTokenCookie(newRawToken);

        return Ok(new LoginResponseDto(token, expiresAt));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var rawToken = Request.Cookies[CookieName];
        if (rawToken is not null)
        {
            await refreshTokenService.RevokeAsync(rawToken);
        }

        Response.Cookies.Delete(CookieName, new CookieOptions { Path = "/api/auth" });
        return Ok();
    }

    private void SetRefreshTokenCookie(string rawToken)
    {
        Response.Cookies.Append(CookieName, rawToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = !env.IsDevelopment(),
            SameSite = env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            Path = "/api/auth",
        });
    }
}