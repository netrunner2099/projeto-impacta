using Credenciamento.Application.Commands.User;

namespace Credenciamento.Application.Handlers.User;

public class RecoverUserPasswordCommandHandler : IRequestHandler<RecoverUserPasswordCommand, RecoverUserPasswordCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IUserService _service;
    public RecoverUserPasswordCommandHandler(
        ILogger<RecoverUserPasswordCommandHandler> logger, 
        IUserService service)
    {
        _logger = logger;
        _service = service;
    }

    public async Task<RecoverUserPasswordCommandResponse> Handle(RecoverUserPasswordCommand request, CancellationToken cancellationToken)
    {
        RecoverUserPasswordCommandResponse response = new();

        try
        {
            response.Success = await _service.SendForgotPasswordAsync(request.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
        }

        return response;
    }
}
