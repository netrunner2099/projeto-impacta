using Credenciamento.Application.Commands.Event;
using FluentValidation;

namespace Credenciamento.Application.Handlers.Event;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, CreateEventCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IEventRepository _repository;
    private readonly IValidator<CreateEventCommand> _validator;

    public CreateEventCommandHandler(
        ILogger<CreateEventCommandHandler> logger,
        IMapper mapper,
        IEventRepository repository,
        IValidator<CreateEventCommand> validator)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
        _validator = validator;
    }

    public async Task<CreateEventCommandResponse> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        CreateEventCommandResponse response = new();

        try
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                response.Errors = validation.Errors.Select(e => e.ErrorMessage).ToArray();
                return response;
            }

            var entity = _mapper.Map<Credenciamento.Domain.Entities.Event>(request);
            entity.Status = (byte)Credenciamento.Domain.Enums.EventStatus.Active;
            entity.CreatedAt = DateTime.UtcNow;

            await _repository.AddAsync(entity);
            response.Success = true;
            response.Message = "Evento cadastrado com sucesso.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
            response.Message = "Erro ao cadastrar evento.";
        }

        return response;
    }
}
