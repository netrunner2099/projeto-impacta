using Credenciamento.Application.Queries.Event;

namespace Credenciamento.Application.Handlers.Event;

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, GetEventByIdQueryResponse>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IEventRepository _repository;

    public GetEventByIdQueryHandler(
        ILogger<GetEventByIdQueryHandler> logger,
        IMapper mapper,
        IEventRepository repository)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<GetEventByIdQueryResponse> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        GetEventByIdQueryResponse response = new();

        try
        {
            var entity = await _repository.GetByIdAsync(request.EventId);
            response.Event = _mapper.Map<EventModel>(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
        }

        return response;
    }
}
