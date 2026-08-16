namespace TimeTracker.Api.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public required int UserId { get; set; }
    public User? User { get; set; }
    public required string TokenHash { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;
}