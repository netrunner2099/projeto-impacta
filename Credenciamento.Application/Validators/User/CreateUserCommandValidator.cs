using Credenciamento.Application.Commands.User;

namespace Credenciamento.Application.Validators.User;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(100).WithMessage("Nome não deve exceder 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Formato de email inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória.")
            .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres.");
    }

    public IEnumerable<string> ValidateCommand(CreateUserCommand command)
    {
        var result = Validate(command);
        return result.Errors.Select(e => e.ErrorMessage);
    }
    public bool IsValid(CreateUserCommand command)
    {
        return Validate(command).IsValid;
    }
}


