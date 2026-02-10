namespace Credenciamento.Application.Handlers.Person;

public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, CreatePersonCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IPersonRepository _repository;
    private readonly IValidator<CreatePersonCommand> _validator;
    public CreatePersonCommandHandler(
        ILogger<CreatePersonCommandHandler> logger,
        IMapper mapper,
        IPersonRepository repository,
        IValidator<CreatePersonCommand> validator)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
        _validator = validator;
    }

    public async Task<CreatePersonCommandResponse> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        CreatePersonCommandResponse returns = new();
        try
        {
            var validation = await _validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                returns.Errors = validation.Errors.Select(e => e.ErrorMessage).ToArray();
                _logger.LogWarning("Handle: validation errors {0}", string.Join(", ", returns.Errors));
            }

            var model = _mapper.Map<Domain.Entities.Person>(request);
            var result = await _repository.AddAsync(model);
            returns = _mapper.Map<CreatePersonCommandResponse>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: error {0}", ex.Message);
        }

        return returns;
    }
}


