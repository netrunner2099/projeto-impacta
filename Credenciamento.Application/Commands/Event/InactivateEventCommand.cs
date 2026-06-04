namespace Credenciamento.Application.Commands.Event;

public class InactivateEventCommand : IRequest<InactivateEventCommandResponse>
{
    public long EventId { get; set; }
}
