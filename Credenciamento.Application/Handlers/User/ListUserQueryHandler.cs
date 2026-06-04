namespace Credenciamento.Application.Handlers.User;

public class ListUserQueryHandler : IRequestHandler<ListUserQuery, ListUserQueryResponse>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository;

    public ListUserQueryHandler(
        ILogger<ListUserQueryHandler> logger,
        IMapper mapper,
        IUserRepository repository)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<ListUserQueryResponse> Handle(ListUserQuery request, CancellationToken cancellationToken)
    {
        ListUserQueryResponse returns = new();

        try
        {
            var users = await _repository.ListAllAsync();

            if (!string.IsNullOrWhiteSpace(request.Name))
                users = users.Where(u => u.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase));

            if (request.Role.HasValue)
                users = users.Where(u => u.Role == request.Role.Value);

            returns.Users = _mapper.Map<List<UserModel>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: error {0}", ex.Message);
        }

        return returns;
    }
}
