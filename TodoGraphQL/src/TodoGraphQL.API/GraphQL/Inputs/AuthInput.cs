namespace TodoGraphQL.API.GraphQL.Inputs;

public record LoginInput(string Email, string Password);
public record RegisterInput(string Email, string Password);