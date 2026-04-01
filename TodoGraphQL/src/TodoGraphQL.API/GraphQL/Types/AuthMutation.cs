using TodoGraphQL.Application.DTOs;
using TodoGraphQL.Application.UseCases.Auth;

namespace TodoGraphQL.API.GraphQL.Types;

[ExtendObjectType("Mutation")]
public class AuthMutation
{
    public async Task<AuthDto> Register(
        string email,
        string password,
        [Service] RegisterUseCase useCase)
        => await useCase.ExecuteAsync(email, password);

    public async Task<AuthDto> Login(
        string email,
        string password,
        [Service] LoginUseCase useCase)
        => await useCase.ExecuteAsync(email, password);
}