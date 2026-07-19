namespace TimeTracker.Api.DTOs;
using System.ComponentModel.DataAnnotations;

public record ClientDto(int Id, string Name, string? ContactEmail, string? Note);

public record CreateClientDto(
    [Required, MinLength(1)] string Name,
    [EmailAddress] string? ContactEmail,
    string? Note
);

public record UpdateClientDto(
    [Required, MinLength(1)] string Name,
    [EmailAddress] string? ContactEmail,
    string? Note
);