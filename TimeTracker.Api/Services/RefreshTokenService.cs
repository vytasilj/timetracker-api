using TimeTracker.Api.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
namespace TimeTracker.Api.Services;
public class RefreshTokenService(AppDbContext db, IConfiguration configuration)
{
    private int ExpiryDays => int.Parse(configuration["Jwt:RefreshTokenExpiryDays"] ?? "30");


    public async Task<string> IssueAsync(int userId)
    {
        var rawToken = GenerateRefreshTokenString(userId);
        await db.SaveChangesAsync();

        return rawToken;
    }

    public async Task<(string Token, int UserId)?> ValidateAndRotateAsync(string refreshToken)
    {
        var tokenHash = RefreshTokenHasher.Hash(refreshToken);
        var existingToken = await db.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (existingToken is null || !existingToken.IsActive)
        {
            return null;
        }

        // Revoke the old token
        existingToken.RevokedAt = DateTime.UtcNow;

        // Issue a new token
        var newRawToken = GenerateRefreshTokenString(existingToken.UserId);

        await db.SaveChangesAsync();

        return (newRawToken, existingToken.UserId);
    }

    public async Task RevokeAsync(string refreshToken)
    {
        var tokenHash = RefreshTokenHasher.Hash(refreshToken);
        var existingToken = await db.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
        if (existingToken is not null && existingToken.RevokedAt is null)
        {
            db.RefreshTokens.Remove(existingToken);
            await db.SaveChangesAsync();
        }
    }
    
    private string GenerateRefreshTokenString(int userId)
    {
        var rawToken = RefreshTokenHasher.GenerateToken();

        db.RefreshTokens.Add(new Models.RefreshToken
        {
            UserId = userId,
            TokenHash = RefreshTokenHasher.Hash(rawToken),
            ExpiresAt = DateTime.UtcNow.AddDays(ExpiryDays)
        });

        return rawToken;
    }
}