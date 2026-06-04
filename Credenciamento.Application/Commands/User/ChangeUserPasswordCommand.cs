namespace Credenciamento.Application.Commands.User;

public class ChangeUserPasswordCommand : IRequest<ChangeUserPasswordCommandResponse>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordConfirm { get; set; }
}