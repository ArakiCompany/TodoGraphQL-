using HotChocolate.Authorization;
using System.Security.Claims;
using TodoGraphQL.Application.DTOs;
using TodoGraphQL.Application.UseCases.Todos;

namespace TodoGraphQL.API.GraphQL.Types;

public class Mutation
{
    [Authorize]
    public async Task<TodoDto> AddTodo(
        string title,
        [Service] AddTodoUseCase useCase,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await useCase.ExecuteAsync(title, userId);
    }

    [Authorize]
    public async Task<TodoDto> CompleteTodo(
        string id,
        [Service] CompleteTodoUseCase useCase,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await useCase.ExecuteAsync(id, userId);
    }

    [Authorize]
    public async Task<bool> DeleteTodo(
        string id,
        [Service] DeleteTodoUseCase useCase,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await useCase.ExecuteAsync(id, userId);
    }
}