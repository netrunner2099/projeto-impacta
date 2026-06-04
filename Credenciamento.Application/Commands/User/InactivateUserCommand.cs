namespace Credenciamento.Application.Commands.User;

public class InactivateUserCommand : IRequest<InactivateUserCommandResponse>
{
    public long UserId { get; set; }
}
