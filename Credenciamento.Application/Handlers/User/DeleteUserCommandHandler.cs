using Credenciamento.Application.Commands.User;

namespace Credenciamento.Application.Handlers.User;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeleteUserCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IUserRepository _repository;

    public DeleteUserCommandHandler(
        ILogger<DeleteUserCommandHandler> logger,
        IUserRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<DeleteUserCommandResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        DeleteUserCommandResponse response = new();

        try
        {
            var success = await _repository.DeleteAsync(request.UserId);
            response.Success = success;
            response.Message = success ? "Usuário excluído com sucesso." : "Usuário não encontrado.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
            response.Message = "Erro ao excluir usuário.";
        }

        return response;
    }
}
