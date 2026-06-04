namespace Credenciamento.Application.Contracts.Event;

public class ListEventQueryResponse
{
    public IEnumerable<EventModel> Events { get; set; } = [];
}
