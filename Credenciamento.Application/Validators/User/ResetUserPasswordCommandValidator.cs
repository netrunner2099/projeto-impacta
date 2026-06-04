using Credenciamento.Application.Commands.User;
using Credenciamento.Application.Interfaces.Global;

namespace Credenciamento.Application.Validators.User;

public class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
{
    private readonly ICacheService _cacheService;
    public ResetUserPasswordCommandValidator(
        ICacheService cacheService)
    {
        _cacheService = cacheService;

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

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token é obrigatório.")
            .MustAsync(async (token, cancellation) =>
            {
                // Verificar se o token existe no cache
                var cacheKey = $"forgot:tokens:{token}";
                return _cacheService.HasKey(cacheKey);
            }).WithMessage("Token inválido ou expirado.");
    }

    public IEnumerable<string> ValidateCommand(ResetUserPasswordCommand command)
    {
        var result = Validate(command);
        return result.Errors.Select(e => e.ErrorMessage);
    }
    public bool IsValid(ResetUserPasswordCommand command)
    {
        return Validate(command).IsValid;
    }
}
