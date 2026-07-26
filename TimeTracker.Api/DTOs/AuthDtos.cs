using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Api.DTOs;

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record LoginResponseDto(string Token, DateTime ExpiresAt);