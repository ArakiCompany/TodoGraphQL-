using FluentValidation;
using TodoGraphQL.API.GraphQL.Inputs;

namespace TodoGraphQL.API.GraphQL.Validators;

public class LoginValidator : AbstractValidator<LoginInput>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email não pode ser vazio.")
            .EmailAddress().WithMessage("Email inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha não pode ser vazia.")
            .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.");
    }
}

public class RegisterValidator : AbstractValidator<RegisterInput>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email não pode ser vazio.")
            .EmailAddress().WithMessage("Email inválido.")
            .MaximumLength(100).WithMessage("Email muito longo.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha não pode ser vazia.")
            .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.")
            .MaximumLength(100).WithMessage("Senha muito longa.")
            .Matches("[A-Z]").WithMessage("A senha deve ter pelo menos uma letra maiúscula.")
            .Matches("[0-9]").WithMessage("A senha deve ter pelo menos um número.");
    }
}