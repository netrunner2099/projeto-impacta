using System.Text.Json;

namespace Credenciamento.Application.Handlers.User;

public class VerifyLoginQueryHandler : IRequestHandler<VerifyLoginQuery, VerifyLoginQueryResponse>
{
    private readonly ILogger _logger;
    private readonly IUserRepository _repository;
    private readonly IPersonRepository _personRepository;
    public VerifyLoginQueryHandler(
        ILogger<VerifyLoginQueryHandler> logger,
        IUserRepository repository,
        IPersonRepository personRepository)
    {
        _logger = logger;
        _repository = repository;
        _personRepository = personRepository;
    }

    public async Task<VerifyLoginQueryResponse> Handle(VerifyLoginQuery request, CancellationToken cancellationToken)
    {
        VerifyLoginQueryResponse returns = null;

        try
        {
            // Consultando usuário
            var user = await _repository.GetByEmailAsync(request.Email);
            if (user is null)
                return returns;

            // Verificando se a senha confere
            if (!CryptHelpers.VerifyHashedPassword(user.Password, request.Password))
                return returns;

            var person = await _personRepository.GetByEmailAsync(user.Email);

            // Criando o token
            var token = JsonSerializer.Serialize(new UserModel
            {
                UserId = user.UserId,
                PersonId = user.PersonId ?? 0,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            }, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault });
            token = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
            returns = new() { Token = token, PersonId = person.PersonId };

            return returns;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
        }

        return returns;
    }
}


