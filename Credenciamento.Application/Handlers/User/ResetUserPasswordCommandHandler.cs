using Credenciamento.Application.Commands.User;
using Credenciamento.Application.Interfaces.Global;
using Org.BouncyCastle.X509;

namespace Credenciamento.Application.Handlers.User;

public class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, ResetUserPasswordCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IUserService _service;
    private readonly IValidator<ResetUserPasswordCommand> _validator;
    private readonly ICacheService _cache;
    public ResetUserPasswordCommandHandler(
        ILogger<ResetUserPasswordCommandHandler> logger,
        IMapper mapper,
        IUserService service,
        IValidator<ResetUserPasswordCommand> validator,
        ICacheService cache)
    {
        _logger = logger;
        _mapper = mapper;
        _service = service;
        _validator = validator;
        _cache = cache;
    }
    public async Task<ResetUserPasswordCommandResponse> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        ResetUserPasswordCommandResponse response = new();

        try
        {
            var validation = await _validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                response.Errors = validation.Errors.Select(e => e.ErrorMessage).ToArray();
                _logger.LogWarning("Handle: validation errors {0}", string.Join(", ", response.Errors));
                return response;
            }

            var model = _mapper.Map<UserModel>(request);
            var email = _cache.GetString($"forgot:tokens:{request.Token}");
            if (!string.IsNullOrEmpty(email))
                model.Email = email;

            response.Success = await _service.ChangePasswordAsync(model);
            _cache.RemoveKey($"forgot:tokens:{request.Token}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Method: {0}", ex.Message);
        }


        return response;
    }
}
