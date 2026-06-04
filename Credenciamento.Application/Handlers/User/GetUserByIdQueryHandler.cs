namespace Credenciamento.Application.Handlers.User;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdQueryResponse>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository;

    public GetUserByIdQueryHandler(
        ILogger<GetUserByIdQueryHandler> logger,
        IMapper mapper,
        IUserRepository repository)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<GetUserByIdQueryResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        GetUserByIdQueryResponse response = new();

        try
        {
            var user = await _repository.GetByIdAsync(request.UserId);
            response.User = _mapper.Map<UserModel>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
        }

        return response;
    }
}
