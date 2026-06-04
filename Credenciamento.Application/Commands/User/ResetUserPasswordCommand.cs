namespace Credenciamento.Application.Commands.User;

public class    ResetUserPasswordCommand : IRequest<ResetUserPasswordCommandResponse>
{
    public string Token { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordConfirm { get; set; }
}