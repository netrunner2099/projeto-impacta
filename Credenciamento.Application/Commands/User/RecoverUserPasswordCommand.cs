namespace Credenciamento.Application.Commands.User;

public class RecoverUserPasswordCommand : IRequest<RecoverUserPasswordCommandResponse>
{
    public string Email { get; set; }
}
