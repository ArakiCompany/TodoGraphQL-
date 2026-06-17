using Microsoft.Extensions.Logging;
using TodoGraphQL.Application.DTOs;
using TodoGraphQL.Domain.Entities;
using TodoGraphQL.Domain.Interfaces;

namespace TodoGraphQL.Application.UseCases.Todos;

public class GenerateTodosUseCase
{
    private readonly IAiService _aiService;
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<GenerateTodosUseCase> _logger;

    public GenerateTodosUseCase(
        IAiService aiService,
        ITodoRepository todoRepository,
        ILogger<GenerateTodosUseCase> logger)
    {
        _aiService = aiService;
        _todoRepository = todoRepository;
        _logger = logger;
    }

    // Só gera sugestões sem salvar
    public async Task<List<string>> SuggestAsync(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
            throw new DomainException("O prompt não pode ser vazio.");

        if (prompt.Length > 200)
            throw new DomainException("O prompt deve ter no máximo 200 caracteres.");

        return await _aiService.GenerateTodosAsync(prompt);
    }

    // Gera e salva direto no banco
    public async Task<List<TodoDto>> GenerateAndSaveAsync(string prompt, string userId)
    {
        var suggestions = await SuggestAsync(prompt);
        var saved = new List<TodoDto>();

        foreach (var title in suggestions)
        {
            var todo = Todo.Create(title, userId);
            var result = await _todoRepository.AddAsync(todo);
            saved.Add(new TodoDto(result.Id, result.Title, result.IsCompleted, result.CreatedAt));
        }

        _logger.LogInformation(
            "Salvou {Count} todos gerados por IA para UserId: {UserId}",
            saved.Count, userId);

        return saved;
    }
}