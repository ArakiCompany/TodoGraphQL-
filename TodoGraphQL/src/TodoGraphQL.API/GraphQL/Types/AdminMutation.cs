using HotChocolate.Authorization;
using TodoGraphQL.Application.UseCases.Admin;

namespace TodoGraphQL.API.GraphQL.Types;

[ExtendObjectType("Mutation")]
public class AdminMutation
{
    [Authorize(Roles = new[] { "Admin" })] // ← só Admin pode acessar
    public async Task<bool> UpdateUserRole(
        string email,
        string role,
        [Service] UpdateUserRoleUseCase useCase)
        => await useCase.ExecuteAsync(email, role);
}