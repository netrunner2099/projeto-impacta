using Credenciamento.Application.Commands.Event;
using FluentValidation;

namespace Credenciamento.Application.Handlers.Event;

public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, UpdateEventCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IEventRepository _repository;
    private readonly IValidator<UpdateEventCommand> _validator;

    public UpdateEventCommandHandler(
        ILogger<UpdateEventCommandHandler> logger,
        IMapper mapper,
        IEventRepository repository,
        IValidator<UpdateEventCommand> validator)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
        _validator = validator;
    }

    public async Task<UpdateEventCommandResponse> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        UpdateEventCommandResponse response = new();

        try
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                response.Errors = validation.Errors.Select(e => e.ErrorMessage).ToArray();
                return response;
            }

            var entity = await _repository.GetByIdAsync(request.EventId);
            if (entity is null)
            {
                response.Message = "Evento não encontrado.";
                return response;
            }

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Local = request.Local;
            entity.Begin = request.Begin;
            entity.End = request.End;
            entity.Price = request.Price;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
            response.Success = true;
            response.Message = "Evento atualizado com sucesso.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
            response.Message = "Erro ao atualizar evento.";
        }

        return response;
    }
}
