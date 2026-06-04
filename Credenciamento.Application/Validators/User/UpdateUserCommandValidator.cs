using Credenciamento.Application.Commands.User;

namespace Credenciamento.Application.Validators.User;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Usuário inválido.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.")
            .MaximumLength(150).WithMessage("E-mail deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Role)
            .GreaterThan((byte)0).WithMessage("Perfil é obrigatório.");
    }
}
