using Credenciamento.Application.Commands.Event;

namespace Credenciamento.Application.Handlers.Event;

public class ActivateEventCommandHandler : IRequestHandler<ActivateEventCommand, ActivateEventCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IEventRepository _repository;

    public ActivateEventCommandHandler(
        ILogger<ActivateEventCommandHandler> logger,
        IEventRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<ActivateEventCommandResponse> Handle(ActivateEventCommand request, CancellationToken cancellationToken)
    {
        ActivateEventCommandResponse response = new();

        try
        {
            var entity = await _repository.GetByIdAsync(request.EventId);
            if (entity is null)
            {
                response.Message = "Evento não encontrado.";
                return response;
            }

            entity.Status = (byte)Credenciamento.Domain.Enums.EventStatus.Active;
            entity.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(entity);
            response.Success = true;
            response.Message = "Evento reativado com sucesso.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
            response.Message = "Erro ao reativar evento.";
        }

        return response;
    }
}
