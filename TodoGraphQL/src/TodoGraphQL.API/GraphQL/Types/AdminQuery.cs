using HotChocolate.Authorization;
using TodoGraphQL.Application.DTOs;
using TodoGraphQL.Application.UseCases.Admin;

namespace TodoGraphQL.API.GraphQL.Types;

[ExtendObjectType("Query")]
public class AdminQuery
{
    [Authorize(Roles = new[] { "Admin" })]
    public async Task<List<UserDto>> GetUsers(
        [Service] GetUsersUseCase useCase)
        => await useCase.ExecuteAsync();
}