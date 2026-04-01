using HotChocolate.Authorization;
using FluentValidation;
using TodoGraphQL.API.GraphQL.Inputs;
using TodoGraphQL.API.GraphQL.Validators;
using TodoGraphQL.Application.UseCases.Admin;

namespace TodoGraphQL.API.GraphQL.Types;

[ExtendObjectType("Mutation")]
public class AdminMutation
{
    [Authorize(Roles = new[] { "Admin" })]
    public async Task<bool> UpdateUserRole(
        UpdateRoleInput input,
        [Service] UpdateUserRoleUseCase useCase,
        [Service] IValidator<UpdateRoleInput> validator)
    {
        await validator.ValidateAndThrowGraphQLAsync(input);
        return await useCase.ExecuteAsync(input.Email, input.Role);
    }
}