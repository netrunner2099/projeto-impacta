namespace Credenciamento.Application.Commands.Event;

public class ActivateEventCommand : IRequest<ActivateEventCommandResponse>
{
    public long EventId { get; set; }
}
