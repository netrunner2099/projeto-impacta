using Credenciamento.Domain.Enums;
using Credenciamento.Shared.Extensions;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Credenciamento.Application.Services.Person;

public class PersonService : IPersonService
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IPersonRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly SmtpOptions _options;
    public PersonService(
        ILogger<PersonService> logger,
        IMapper mapper,
        IPersonRepository repository,
        IUserRepository userRepository,
        IOptions<SmtpOptions> options)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
        _userRepository = userRepository;
        _options = options.Value;
    }

    public async Task<PersonModel> AddAsync(PersonModel model)
    {
        try
        {
            model.Name = model.Name.ToTitleCase();
            model.Email = model.Email.ToLower();
            model.Address = model.Address.ToTitleCase();
            model.Neighborhood = model.Neighborhood.ToTitleCase();
            model.City = model.City.ToTitleCase();
            model.State = model.State.ToUpper();
            model.CreatedAt = DateTime.UtcNow;
            model.Status = (byte)PersonStatus.Active;
            model.Document = model.Document.MaskRemove();
            model.ZipCode = model.ZipCode.MaskRemove();
            var personResult = await _repository.AddAsync(_mapper.Map<Domain.Entities.Person>(model));
            if (personResult is null)
            {
                _logger.LogWarning("Falha ao criar pessoa para {0}", model.Email);
                return null;
            }
                
            var password = CryptHelpers.RandomPasswordGenerate(8, true, true, true, false, false);
            var personModel = _mapper.Map<PersonModel>(personResult);
            var userResult = await AddUserAsync(personModel, password);
            if (userResult is null)
            {
                _logger.LogWarning("Falha ao criar usuário para {0}", personModel.Email);
                return null;
            }
            
            var sendPasswordResult = await SendPasswordAsync(personModel.Email, password);
            if (!sendPasswordResult)
            {
                
                return null;
            }
            
            return _mapper.Map<PersonModel>(personResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddAsync: {0}", ex.Message);
        }

        return null;
    }

    private async Task<PersonModel> AddUserAsync(PersonModel model, string password)
    {
        try
        {
            var user = new User
            {
                PersonId = model.PersonId,
                Name = model.Name,
                Email = model.Email,
                Password = CryptHelpers.HashPassword(password),
                Role = (byte)UserRole.User,
                Status = (byte)UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };
            var userResult = await _userRepository.AddAsync(user);
            if (userResult is null)
                return null;

            return model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync: {0}", ex.Message);
        }
        return null;
    }

    private async Task<bool> SendPasswordAsync(string email, string password)
    {
        try
        {
            var mime = new MimeMessage();
            mime.From.Add(new MailboxAddress("", _options.Sender));
            string[] toAddresses = email.Replace(" ", "").Split(';');
            foreach (var to in toAddresses)
            {
                mime.To.Add(new MailboxAddress("", to));
            }   
            mime.Subject = "Credenciamento - Senha de Acesso";

            var message = new StringBuilder();
            message.AppendLine("Seguem abaixo os dados de acesso a sua conta:<br>");
            message.AppendLine($"Email: {email}<br>");
            message.AppendLine($"Senha: {password}<br>");
            message.AppendLine("Recomendamos que altere sua senha após o primeiro acesso.");

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message.ToString()
            };
            mime.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                {
                    return true;
                };
                client.Connect(_options.Host, _options.Port, false);
                await client.SendAsync(mime);
                await client.DisconnectAsync(true);

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SendPasswordAsync: {0}", ex.Message);
        }
        return false;
    }
}