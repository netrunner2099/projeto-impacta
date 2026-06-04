using Credenciamento.Application.Commands.User;

namespace Credenciamento.Application.Handlers.User;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, ActivateUserCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IUserRepository _repository;

    public ActivateUserCommandHandler(
        ILogger<ActivateUserCommandHandler> logger,
        IUserRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<ActivateUserCommandResponse> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        ActivateUserCommandResponse response = new();

        try
        {
            var user = await _repository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                response.Message = "Usuário não encontrado.";
                return response;
            }

            user.Status = (byte)Credenciamento.Domain.Enums.UserStatus.Active;
            await _repository.UpdateAsync(user);
            response.Success = true;
            response.Message = "Usuário reativado com sucesso.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
            response.Message = "Erro ao reativar usuário.";
        }

        return response;
    }
}
