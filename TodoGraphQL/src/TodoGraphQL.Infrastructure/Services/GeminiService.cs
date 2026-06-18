using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TodoGraphQL.Domain.Interfaces;

namespace TodoGraphQL.Infrastructure.Services;

public class GeminiService : IAiService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly ILogger<GeminiService> _logger;

    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    public GeminiService(IConfiguration config, ILogger<GeminiService> logger)
    {
        _http = new HttpClient();
        _apiKey = config["Gemini:ApiKey"]
            ?? throw new InvalidOperationException("Gemini API key não configurada corretamente.");
        _logger = logger;
    }

 public async Task<List<string>> GenerateTodosAsync(string prompt)
{
    _logger.LogInformation("Gerando todos com Gemini | Prompt: {Prompt}", prompt);

    var systemPrompt = $$"""
        Você é um assistente que gera listas de tarefas práticas e objetivas.
        
        O usuário quer criar uma lista de tarefas sobre: "{{prompt}}"
        
        Gere entre 5 e 10 tarefas específicas, práticas e acionáveis.
        
        IMPORTANTE: Responda APENAS com um JSON válido no seguinte formato, sem texto adicional:
        {
          "todos": [
            "Tarefa 1",
            "Tarefa 2",
            "Tarefa 3"
          ]
        }
        """;

    var requestBody = new
    {
        contents = new[]
        {
            new
            {
                parts = new[]
                {
                    new { text = systemPrompt }
                }
            }
        },
        generationConfig = new
        {
            temperature = 0.7,
            maxOutputTokens = 1024,
        }
    };

    var url = $"{BaseUrl}?key={_apiKey}";
    var json = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    try
    {
        var response = await _http.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Erro na API do Gemini. Status: {Status}, Resposta: {Error}", response.StatusCode, errorContent);
            throw new Exception($"Erro na API do Gemini: {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Log seguro para verificar a estrutura da resposta completa se necessário
        var text = result
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "{}";

        // Limpa possíveis marcações de markdown que o modelo teima em colocar
        text = text
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();

        _logger.LogInformation("Texto limpo recebido do Gemini: {Response}", text);

        using (var parsedJson = JsonDocument.Parse(text))
        {
            if (parsedJson.RootElement.TryGetProperty("todos", out var todosArray) && todosArray.ValueKind == JsonValueKind.Array)
            {
                var todos = todosArray
                    .EnumerateArray()
                    .Select(t => t.GetString() ?? "")
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToList();

                _logger.LogInformation("Gerou {Count} todos com sucesso.", todos.Count);
                return todos;
            }
        }
        
        _logger.LogWarning("O JSON retornado não continha a propriedade 'todos' no formato esperado. Retornando lista vazia.");
        return new List<string>();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Falha crítica no GeminiService ao processar requisição.");
        throw; // Ou retorne uma lista vazia/erro amigável se preferir não quebrar a mutation
    }
}}