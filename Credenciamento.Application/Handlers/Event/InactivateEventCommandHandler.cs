using Credenciamento.Application.Commands.Event;

namespace Credenciamento.Application.Handlers.Event;

public class InactivateEventCommandHandler : IRequestHandler<InactivateEventCommand, InactivateEventCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IEventRepository _repository;

    public InactivateEventCommandHandler(
        ILogger<InactivateEventCommandHandler> logger,
        IEventRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<InactivateEventCommandResponse> Handle(InactivateEventCommand request, CancellationToken cancellationToken)
    {
        InactivateEventCommandResponse response = new();

        try
        {
            var entity = await _repository.GetByIdAsync(request.EventId);
            if (entity is null)
            {
                response.Message = "Evento não encontrado.";
                return response;
            }

            entity.Status = (byte)Credenciamento.Domain.Enums.EventStatus.Inactive;
            entity.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(entity);
            response.Success = true;
            response.Message = "Evento inativado com sucesso.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
            response.Message = "Erro ao inativar evento.";
        }

        return response;
    }
}
