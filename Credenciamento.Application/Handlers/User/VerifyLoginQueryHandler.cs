using Credenciamento.Application.Interfaces.Global;
using System.Text.Json;

namespace Credenciamento.Application.Handlers.User;

public class VerifyLoginQueryHandler : IRequestHandler<VerifyLoginQuery, VerifyLoginQueryResponse>
{
    private readonly ILogger _logger;
    private readonly IUserService _userService;
    private readonly IPersonRepository _personRepository;
    private readonly ICacheService _cache;
    public VerifyLoginQueryHandler(
        ILogger<VerifyLoginQueryHandler> logger,
        IUserService userService,
        IPersonRepository personRepository,
        ICacheService cache)
    {
        _logger = logger;
        _userService = userService;
        _personRepository = personRepository;
        _cache = cache;
    }

    public async Task<VerifyLoginQueryResponse> Handle(VerifyLoginQuery request, CancellationToken cancellationToken)
    {
        VerifyLoginQueryResponse returns = null;

        try
        {
            // Consultando usuário
            var user = await _userService.LoginAsync(request.Email, request.Password);
            if (user is null)
                return returns;

            var person = await _personRepository.GetByEmailAsync(user.Email);

            var userModel = new UserModel
            {
                UserId = user.UserId,
                PersonId = user.PersonId ?? 0,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
            var content = JsonSerializer.Serialize(userModel, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault });
            var token = StringHelpers.ToBase64(CryptHelpers.HashGenerate(content, "md5").ToLower());
            _cache.SetObject($"user:{CryptHelpers.HashGenerate(content, "md5").ToLower()}", userModel, 30);
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


