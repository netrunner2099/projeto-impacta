namespace Credenciamento.Application.Queries.Event;

public class GetEventQuery : IRequest<GetEventQueryResponse?>
{
    public long EventId { get; set; }
}


