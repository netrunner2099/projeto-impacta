namespace Credenciamento.Application.Queries.Event;

public class GetEventByIdQuery : IRequest<GetEventByIdQueryResponse>
{
    public long EventId { get; set; }
}
