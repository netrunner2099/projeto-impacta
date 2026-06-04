namespace Credenciamento.Application.Commands.User;

public class CreateUserCommand : IRequest<CreateUserCommandResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PasswordConfirm { get; set; } = string.Empty;
    public byte Role { get; set; }
}
