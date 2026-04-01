using FluentValidation;
using TodoGraphQL.API.GraphQL.Inputs;
using TodoGraphQL.API.GraphQL.Validators;
using TodoGraphQL.Application.DTOs;
using TodoGraphQL.Application.UseCases.Auth;

namespace TodoGraphQL.API.GraphQL.Types;

[ExtendObjectType("Mutation")]
public class AuthMutation
{
    public async Task<AuthDto> Register(
        RegisterInput input,
        [Service] RegisterUseCase useCase,
        [Service] IValidator<RegisterInput> validator)
    {
        await validator.ValidateAndThrowGraphQLAsync(input);
        return await useCase.ExecuteAsync(input.Email, input.Password);
    }

    public async Task<AuthDto> Login(
        LoginInput input,
        [Service] LoginUseCase useCase,
        [Service] IValidator<LoginInput> validator)
    {
        await validator.ValidateAndThrowGraphQLAsync(input);
        return await useCase.ExecuteAsync(input.Email, input.Password);
    }
}