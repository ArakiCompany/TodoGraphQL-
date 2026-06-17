using Microsoft.Extensions.DependencyInjection;
using TodoGraphQL.Application.Interfaces;
using TodoGraphQL.Domain.Interfaces;
using TodoGraphQL.Infrastructure.Persistence;
using TodoGraphQL.Infrastructure.Services;

namespace TodoGraphQL.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<MongoDbContext>();
        services.AddScoped<ITodoRepository, TodoRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFinanceRepository, FinanceRepository>(); // ← novo
        services.AddScoped<IAiService, GeminiService>();
        services.AddScoped<ITokenService, TokenService>();
        return services;
    }
}