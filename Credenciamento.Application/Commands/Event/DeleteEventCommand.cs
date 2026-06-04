namespace Credenciamento.Application.Commands.Event;

public class DeleteEventCommand : IRequest<DeleteEventCommandResponse>
{
    public long EventId { get; set; }
}
