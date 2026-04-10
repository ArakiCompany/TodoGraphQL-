namespace TodoGraphQL.Application.DTOs;

public record UserDto(
    string Id,
    string Email,
    string Role,
    DateTime CreatedAt
);