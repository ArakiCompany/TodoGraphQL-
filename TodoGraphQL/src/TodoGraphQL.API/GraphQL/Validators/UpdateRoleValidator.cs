using FluentValidation;
using TodoGraphQL.API.GraphQL.Inputs;
using TodoGraphQL.Domain.Entities;

namespace TodoGraphQL.API.GraphQL.Validators;

public class UpdateRoleValidator : AbstractValidator<UpdateRoleInput>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email não pode ser vazio.")
            .EmailAddress().WithMessage("Email inválido.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("A role não pode ser vazia.")
            .Must(r => Enum.TryParse<UserRole>(r, true, out _))
            .WithMessage($"Role inválida. Use: {string.Join(", ", Enum.GetNames<UserRole>())}");
    }
}