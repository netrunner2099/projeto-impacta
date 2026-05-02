using Credenciamento.Application.Commands.User;

namespace Credenciamento.Application.Validators.User;

public class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommandValidator()
    {
        // Validação da senha
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres.")
            .Matches(@"[A-Za-z]").WithMessage("A senha deve conter letras.")
            .Matches(@"[0-9]").WithMessage("A senha deve conter números.");

        // Validação da confirmação de senha
        RuleFor(x => x.PasswordConfirm)
            .NotEmpty().WithMessage("Confirmação de senha é obrigatória.")
            .Equal(x => x.Password).WithMessage("As senhas não coincidem.");
    }

    public IEnumerable<string> ValidateCommand(ChangeUserPasswordCommand command)
    {
        var result = Validate(command);
        return result.Errors.Select(e => e.ErrorMessage);
    }
    public bool IsValid(ChangeUserPasswordCommand command)
    {
        return Validate(command).IsValid;
    }
}
