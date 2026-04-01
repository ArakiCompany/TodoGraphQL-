using FluentValidation;
using TodoGraphQL.API.GraphQL.Inputs;

namespace TodoGraphQL.API.GraphQL.Validators;

public class AddTodoValidator : AbstractValidator<AddTodoInput>
{
    public AddTodoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título não pode ser vazio.")
            .MinimumLength(3).WithMessage("O título deve ter pelo menos 3 caracteres.")
            .MaximumLength(200).WithMessage("O título deve ter no máximo 200 caracteres.")
            .Must(t => !string.IsNullOrWhiteSpace(t)).WithMessage("O título não pode conter apenas espaços.");
    }
}