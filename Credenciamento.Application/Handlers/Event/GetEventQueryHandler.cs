namespace Credenciamento.Application.Handlers.Event;

public class GetEventQueryHandler : IRequestHandler<GetEventQuery, GetEventQueryResponse?>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IEventRepository _repository;
    public GetEventQueryHandler(
        ILogger<GetEventQueryHandler> logger,
        IMapper mapper,
        IEventRepository repository)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<GetEventQueryResponse> Handle(GetEventQuery request, CancellationToken cancellationToken)
    {
        GetEventQueryResponse returns = null;

        try
        {
            returns = _mapper.Map<GetEventQueryResponse>(await _repository.GetByIdAsync(request.EventId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: error {0}", ex.Message);
        }

        return returns;
    }
}


