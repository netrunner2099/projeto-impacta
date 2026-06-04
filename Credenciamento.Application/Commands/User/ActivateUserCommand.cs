namespace Credenciamento.Application.Commands.User;

public class ActivateUserCommand : IRequest<ActivateUserCommandResponse>
{
    public long UserId { get; set; }
}
