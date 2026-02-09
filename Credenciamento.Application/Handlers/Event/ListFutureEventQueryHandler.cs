
using Microsoft.Extensions.Logging;

namespace Credenciamento.Application.Handlers.Event;

public class ListFutureEventQueryHandler : IRequestHandler<ListFutureEventQuery, ListFutureEventQueryResponse>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IEventRepository _repository;
    public ListFutureEventQueryHandler(
        ILogger<ListFutureEventQueryHandler> logger,
        IMapper mapper,
        IEventRepository repository)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<ListFutureEventQueryResponse> Handle(ListFutureEventQuery request, CancellationToken cancellationToken)
    {
        ListFutureEventQueryResponse returns = new();

        try
        {
            returns.Events = _mapper.Map<List<EventModel>>(await _repository.ListFutureAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while listing future events");
        }

        return returns;
    }
}


