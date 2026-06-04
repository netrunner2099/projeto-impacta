using Credenciamento.Application.Commands.User;
using Credenciamento.Shared.Helpers;

namespace Credenciamento.Application.Handlers.User;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository;
    private readonly IValidator<CreateUserCommand> _validator;

    public CreateUserCommandHandler(
        ILogger<CreateUserCommandHandler> logger,
        IMapper mapper,
        IUserRepository repository,
        IValidator<CreateUserCommand> validator)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
        _validator = validator;
    }

    public async Task<CreateUserCommandResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        CreateUserCommandResponse response = new();

        try
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                response.Errors = validation.Errors.Select(e => e.ErrorMessage).ToArray();
                return response;
            }

            if (await _repository.EmailExistsAsync(request.Email))
            {
                response.Errors = ["Este e-mail já está cadastrado."];
                return response;
            }

            var user = _mapper.Map<Credenciamento.Domain.Entities.User>(request);
            user.Password = CryptHelpers.HashPassword(request.Password);
            user.Status = (byte)Credenciamento.Domain.Enums.UserStatus.Active;
            user.CreatedAt = DateTime.Now;

            await _repository.AddAsync(user);
            response.Success = true;
            response.Message = "Usuário cadastrado com sucesso.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
            response.Message = "Erro ao cadastrar usuário.";
        }

        return response;
    }
}
