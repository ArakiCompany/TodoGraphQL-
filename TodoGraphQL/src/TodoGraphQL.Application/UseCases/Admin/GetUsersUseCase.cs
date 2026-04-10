using TodoGraphQL.Application.DTOs;
using TodoGraphQL.Domain.Interfaces;

namespace TodoGraphQL.Application.UseCases.Admin;

public class GetUsersUseCase
{
    private readonly IUserRepository _repository;

    public GetUsersUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<UserDto>> ExecuteAsync()
    {
        var users = await _repository.GetAllAsync();

        return users.Select(u => new UserDto(
            u.Id,
            u.Email,
            u.Role.ToString(),
            u.CreatedAt
        )).ToList();
    }
}