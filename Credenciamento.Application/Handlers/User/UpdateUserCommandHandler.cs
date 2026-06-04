using Credenciamento.Application.Commands.User;

namespace Credenciamento.Application.Handlers.User;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdateUserCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository;
    private readonly IValidator<UpdateUserCommand> _validator;

    public UpdateUserCommandHandler(
        ILogger<UpdateUserCommandHandler> logger,
        IMapper mapper,
        IUserRepository repository,
        IValidator<UpdateUserCommand> validator)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
        _validator = validator;
    }

    public async Task<UpdateUserCommandResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        UpdateUserCommandResponse response = new();

        try
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                response.Errors = validation.Errors.Select(e => e.ErrorMessage).ToArray();
                return response;
            }

            var user = await _repository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                response.Errors = ["Usuário não encontrado."];
                return response;
            }

            user.Name = request.Name;
            user.Email = request.Email;
            user.Role = request.Role;

            await _repository.UpdateAsync(user);
            response.Success = true;
            response.Message = "Usuário atualizado com sucesso.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
            response.Message = "Erro ao atualizar usuário.";
        }

        return response;
    }
}
