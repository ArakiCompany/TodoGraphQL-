using FluentValidation;

namespace TodoGraphQL.API.GraphQL.Validators;

public static class ValidationExtensions
{
    public static async Task ValidateAndThrowGraphQLAsync<T>(
        this IValidator<T> validator,
        T instance)
    {
        var result = await validator.ValidateAsync(instance);

        if (!result.IsValid)
        {
            var errors = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new GraphQLException(errors);
        }
    }
}