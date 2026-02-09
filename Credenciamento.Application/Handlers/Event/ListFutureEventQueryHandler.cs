
namespace Credenciamento.Application.Handlers.Event;

public class ListFutureEventQueryHandler : IRequestHandler<ListFutureEventQuery, ListFutureEventQueryResponse>
{
    private readonly IEventRepository _repository;
    public ListFutureEventQueryHandler(IEventRepository repository)
    {
        _repository = repository;
    }

    public Task<ListFutureEventQueryResponse> Handle(ListFutureEventQuery request, CancellationToken cancellationToken)
    {
        try
        {

        }
        catch (Exception ex)
        {

        }
    }
}


