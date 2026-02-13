using Credenciamento.Application.Queries.Person;

namespace Credenciamento.Application.Handlers.Person;

public class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, GetPersonQueryResponse?>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IPersonRepository _repository;
    public GetPersonQueryHandler(
        ILogger<GetPersonQueryHandler> logger,
        IMapper mapper,
        IPersonRepository repository)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<GetPersonQueryResponse> Handle(GetPersonQuery request, CancellationToken cancellationToken)
    {
        GetPersonQueryResponse returns = null;

        try
        {
            returns = _mapper.Map<GetPersonQueryResponse>(await _repository.GetByIdAsync(request.PersonId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
        }

        return returns;
    }
}


