namespace TodoGraphQL.Domain.Interfaces;

public interface IAiService
{
    Task<List<string>> GenerateTodosAsync(string prompt);
}