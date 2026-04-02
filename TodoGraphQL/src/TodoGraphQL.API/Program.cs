using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TodoGraphQL.API.Application;
using TodoGraphQL.API.GraphQL.Types;
using TodoGraphQL.Application.UseCases.Admin;
using TodoGraphQL.Infrastructure;
using FluentValidation;
using TodoGraphQL.API.GraphQL.Validators;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using TodoGraphQL.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .SetIsOriginAllowed(origin => origin.Contains("vercel.app"))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Camadas
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddValidatorsFromAssemblyContaining<AddTodoValidator>();

builder.Services.AddScoped<UpdateUserRoleUseCase>();

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddTypeExtension<AuthMutation>()
    .AddTypeExtension<AdminMutation>()
    .AddAuthorization()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);


// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429; // Too Many Requests

    // Política geral — 60 req/min por IP
    options.AddPolicy("general", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    // Política para auth — 5 req/min por IP
    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    // Resposta customizada quando limite é atingido
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(
            """{"errors":[{"message":"Muitas requisições. Tente novamente em instantes."}]}""",
            cancellationToken);
    };
});

var app = builder.Build();

app.UseRateLimiter();
app.UseMiddleware<AuthRateLimitMiddleware>();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL().RequireRateLimiting("general");
app.Run();