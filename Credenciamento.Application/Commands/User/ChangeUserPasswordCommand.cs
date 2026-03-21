namespace Credenciamento.Application.Commands.User;

public class ChangeUserPasswordCommand
{
    public string Email { get; set; }
    public string Password { get; set; }
}