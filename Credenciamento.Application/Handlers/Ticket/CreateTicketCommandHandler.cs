namespace Credenciamento.Application.Handlers.Ticket;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, CreateTicketCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly ITicketRepository _repository;
    private readonly IEventRepository _eventRepository;
    private readonly IValidator<CreateTicketCommand> _validator;
    public CreateTicketCommandHandler(
        ILogger<CreateTicketCommandHandler> logger, 
        IMapper mapper,
        ITicketRepository repository,
        IEventRepository eventRepository,
        IValidator<CreateTicketCommand> validator)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
        _eventRepository = eventRepository;
        _validator = validator;
    }

    public async Task<CreateTicketCommandResponse> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        CreateTicketCommandResponse returns = null;

        try
        {
            var validation = await _validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Handle: {0}", string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
                return new()
                {
                    ErrorMessages = validation.Errors.Select(x => x.ErrorMessage).ToList()
                };
            }

            var evento = await _eventRepository.GetByIdAsync(request.EventId);
            if (evento is null)
            {
                _logger.LogWarning("Handle: {0}", "Evento não encontrado");
                return new()
                {
                    ErrorMessages = new List<string> { "Evento não encontrado" }
                };
            }

            var entity = _mapper.Map<Domain.Entities.Ticket>(request);
            entity.CreatedAt = DateTime.UtcNow;
            entity.Price = evento.Price;
            var ticket =  await _repository.AddAsync(entity);
            if (ticket is null)
            {
                _logger.LogWarning("Handle: {0}", "Erro ao inserir ticket");
                return new()
                {
                    ErrorMessages = new List<string> { "Erro ao inserir ticket" }
                };
            }
                
            returns = _mapper.Map<CreateTicketCommandResponse>(ticket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
        }

        return returns;
    }
}


