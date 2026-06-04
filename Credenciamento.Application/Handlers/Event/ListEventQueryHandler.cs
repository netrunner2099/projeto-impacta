using Credenciamento.Application.Queries.Event;

namespace Credenciamento.Application.Handlers.Event;

public class ListEventQueryHandler : IRequestHandler<ListEventQuery, ListEventQueryResponse>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IEventRepository _repository;

    public ListEventQueryHandler(
        ILogger<ListEventQueryHandler> logger,
        IMapper mapper,
        IEventRepository repository)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<ListEventQueryResponse> Handle(ListEventQuery request, CancellationToken cancellationToken)
    {
        ListEventQueryResponse response = new();

        try
        {
            var events = await _repository.ListAllAsync();

            if (request.OnlyFuture)
                events = events.Where(e => e.End >= DateTime.UtcNow);

            if (!string.IsNullOrWhiteSpace(request.Name))
                events = events.Where(e => e.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase));

            if (request.Status.HasValue)
                events = events.Where(e => e.Status == request.Status.Value);

            response.Events = _mapper.Map<IEnumerable<EventModel>>(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
        }

        return response;
    }
}
