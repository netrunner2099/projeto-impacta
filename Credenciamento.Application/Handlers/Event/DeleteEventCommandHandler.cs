using Credenciamento.Application.Commands.Event;

namespace Credenciamento.Application.Handlers.Event;

public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, DeleteEventCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IEventRepository _repository;

    public DeleteEventCommandHandler(
        ILogger<DeleteEventCommandHandler> logger,
        IEventRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<DeleteEventCommandResponse> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        DeleteEventCommandResponse response = new();

        try
        {
            var entity = await _repository.GetByIdAsync(request.EventId);
            if (entity is null)
            {
                response.Message = "Evento não encontrado.";
                return response;
            }

            entity.Status = (byte)Credenciamento.Domain.Enums.EventStatus.Deleted;
            entity.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(entity);
            response.Success = true;
            response.Message = "Evento excluído com sucesso.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
            response.Message = "Erro ao excluir evento.";
        }

        return response;
    }
}
