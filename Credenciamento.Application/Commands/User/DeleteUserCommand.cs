namespace Credenciamento.Application.Commands.User;

public class DeleteUserCommand : IRequest<DeleteUserCommandResponse>
{
    public long UserId { get; set; }
}
