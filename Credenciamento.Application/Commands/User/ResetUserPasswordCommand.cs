namespace Credenciamento.Application.Commands.User;

public class ResetUserPasswordCommand
{
    public string Email { get; set; }
    public string Password { get; set; }
}