using Credenciamento.Application.Commands.User;

namespace Credenciamento.Application.Handlers.User;

public class InactivateUserCommandHandler : IRequestHandler<InactivateUserCommand, InactivateUserCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IUserRepository _repository;

    public InactivateUserCommandHandler(
        ILogger<InactivateUserCommandHandler> logger,
        IUserRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<InactivateUserCommandResponse> Handle(InactivateUserCommand request, CancellationToken cancellationToken)
    {
        InactivateUserCommandResponse response = new();

        try
        {
            var user = await _repository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                response.Message = "Usuário não encontrado.";
                return response;
            }

            user.Status = (byte)Credenciamento.Domain.Enums.UserStatus.Inactive;
            await _repository.UpdateAsync(user);
            response.Success = true;
            response.Message = "Usuário inativado com sucesso.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
            response.Message = "Erro ao inativar usuário.";
        }

        return response;
    }
}
