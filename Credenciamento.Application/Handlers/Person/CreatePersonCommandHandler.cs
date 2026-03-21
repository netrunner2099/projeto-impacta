namespace Credenciamento.Application.Handlers.Person;

public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, CreatePersonCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IPersonService _service;
    private readonly IValidator<CreatePersonCommand> _validator;
    public CreatePersonCommandHandler(
        ILogger<CreatePersonCommandHandler> logger,
        IMapper mapper,
        IPersonService service,
        IValidator<CreatePersonCommand> validator)
    {
        _logger = logger;
        _mapper = mapper;
        _service = service;
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

            var model = _mapper.Map<PersonModel>(request);
            return _mapper.Map<CreatePersonCommandResponse>(await _service.AddAsync(model));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: error {0}", ex.Message);
        }

        return returns;
    }
}


