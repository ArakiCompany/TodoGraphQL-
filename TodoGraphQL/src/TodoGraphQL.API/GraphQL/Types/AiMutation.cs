using HotChocolate.Authorization;
using System.Security.Claims;
using TodoGraphQL.Application.DTOs;
using TodoGraphQL.Application.UseCases.Todos;

namespace TodoGraphQL.API.GraphQL.Types;

[ExtendObjectType("Mutation")]
public class AiMutation
{
    // Retorna sugestões sem salvar — usuário escolhe quais quer
    [Authorize]
    public async Task<List<string>> SuggestTodos(
        string prompt,
        [Service] GenerateTodosUseCase useCase)
        => await useCase.SuggestAsync(prompt);

    // Gera e salva todos de uma vez
    [Authorize]
    public async Task<List<TodoDto>> GenerateAndSaveTodos(
        string prompt,
        [Service] GenerateTodosUseCase useCase,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await useCase.GenerateAndSaveAsync(prompt, userId);
    }
}