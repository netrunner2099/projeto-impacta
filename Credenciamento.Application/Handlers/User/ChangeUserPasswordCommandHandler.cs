using Credenciamento.Application.Commands.User;

namespace Credenciamento.Application.Handlers.User;

public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, ChangeUserPasswordCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IUserService _service;
    private readonly IValidator<ChangeUserPasswordCommand> _validator;
    public ChangeUserPasswordCommandHandler(
        ILogger<ChangeUserPasswordCommandHandler> logger,
        IMapper mapper,
        IUserService service,
        IValidator<ChangeUserPasswordCommand> validator)
    {
        _logger = logger;
        _mapper = mapper;
        _service = service;
        _validator = validator;
    }

    public async Task<ChangeUserPasswordCommandResponse> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        ChangeUserPasswordCommandResponse response = new();

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
            response.Success = await _service.ChangePasswordAsync(model);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Method: {0}", ex.Message);
        }

        return response;
    }
}