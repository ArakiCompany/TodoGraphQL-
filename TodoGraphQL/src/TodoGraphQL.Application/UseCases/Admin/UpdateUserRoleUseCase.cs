using Microsoft.Extensions.Logging;
using TodoGraphQL.Domain.Entities;
using TodoGraphQL.Domain.Interfaces;

namespace TodoGraphQL.Application.UseCases.Admin;

public class UpdateUserRoleUseCase
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UpdateUserRoleUseCase> _logger;

    public UpdateUserRoleUseCase(IUserRepository repository, ILogger<UpdateUserRoleUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> ExecuteAsync(string email, string role)
    {
        var user = await _repository.FindByEmailAsync(email)
            ?? throw new DomainException("Usuário não encontrado.");

        if (!Enum.TryParse<UserRole>(role, true, out var userRole))
            throw new DomainException($"Role inválida. Use: {string.Join(", ", Enum.GetNames<UserRole>())}");

        await _repository.UpdateRoleAsync(user.Id, userRole);

        _logger.LogInformation(
            "Role atualizada: {Email} → {Role}", email, role);

        return true;
    }
}