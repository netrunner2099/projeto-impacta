using Credenciamento.Application.Interfaces.Global;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Credenciamento.Application.Services;

public class UserService : IUserService
{
    private readonly ILogger _logger;
    private readonly ICacheService _cache;
    private readonly IUserRepository _repository;
    private readonly SmtpOptions _options;

    private const int otpExpirationMinutes = 10;
    private const string otpCacheKey = "opt:codes";
    private const string tokenCache = "forgot:tokens";
    private readonly string baseUrl;
    public UserService(
        ILogger<UserService> logger,
        ICacheService cache,
        IUserRepository repository,
        IOptions<SmtpOptions> options,
        IConfiguration configuration)
    {
        _logger = logger;
        _cache = cache;
        _repository = repository;
        _options = options.Value;
        baseUrl = $"{configuration["Environment:BaseUrl"]}";
    }

    public async Task<User> LoginAsync(string email, string password)
    {
        try
        {
            var user = await _repository.GetByEmailAsync(email);
            if (user is null)
            {
                _logger.LogWarning("LoginAsync: Usuário não localizado: {0}", email);
                return null;
            }

            if (!CryptHelpers.VerifyHashedPassword(user.Password, password) && !ValidateOtp(password, user.UserId))
            {
                _logger.LogWarning("LoginAsync: Senha inválida para o usuário: {0}", email);
                return null;
            }
            
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LoginAsync: {0}", ex.Message);
            return null;
        }
    }

    public async Task<bool> GenerateOnetTimePasswordAsync(string email)
    {
        try
        {
            var user = await _repository.GetByEmailAsync(email);
            if (user is null)
            {
                _logger.LogWarning("GenerateOnetTimePasswordAsync: Usuário não localizado: {0}", email);
                return false;
            }

            var otpCode = CryptHelpers.RandomPasswordGenerate(8, true, true, true, false, false);
            var result = this.InsertOtp(otpCode, user.UserId);
            if (result)
                result = await this.SendPasswordAsync(user.Email, otpCode);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GenerateOnetTimePasswordAsync: {0}", ex.Message);
            return false;
        }
    }

    public async Task<bool> SendForgotPasswordAsync(string email)
    {
        try
        {
            var user = await _repository.GetByEmailAsync(email);
            if (user is null)
            {
                _logger.LogWarning("SendForgotPasswordAsync: Usuário não localizado: {0}", email);
                return false;
            }

            var token = CryptHelpers.HashGenerate($"{user.Email}:{Guid.NewGuid()}", "md5").ToLower().Substring(0, 8);
            _cache.SetString($"{tokenCache}:{token}", user.Email, 10);

            return await this.SendForgotenAsync(user.Email, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SendForgotPasswordAsync: {0}", ex.Message);
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(UserModel model)
    {
        try
        {
            var local = await _repository.GetByEmailAsync(model.Email);
            if(local is null)
            {
                _logger.LogWarning("ChangePasswordAsync: Usuário não encontrado: {0}", model.Email);
                return false;
            }

            var password = CryptHelpers.HashPassword(model.Password);
            local.Password = password;
            local.UpdatedAt = DateTime.Now;
            return await _repository.UpdateAsync(local) is not null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ResetPasswordAsync: {0}", ex.Message);
            return false;
        }
    }
    #region Private Methods
    private bool InsertOtp(string otpCode, long userId)
    {
        try
        {
            List<OtpCodeObject> otps = new();
            if (_cache.HasKey(otpCacheKey))
                otps = _cache.GetObject<List<OtpCodeObject>>(otpCacheKey);

            otps.Add(new OtpCodeObject
            {
                UserId = userId,
                OtpCode = otpCode,
                Expiration = DateTime.UtcNow.AddMinutes(otpExpirationMinutes)
            });

            return _cache.SetObject(otpCacheKey, otps);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InsertOtp: {0}", ex.Message);
            return false;
        }
    }

    private bool ValidateOtp(string otpCode, long userId)
    {
        try
        {
            if (!_cache.HasKey(otpCacheKey))
                return false;

            var otps = _cache.GetObject<List<OtpCodeObject>>(otpCacheKey);
            var otp = otps.FirstOrDefault(o => o.UserId == userId && o.OtpCode == otpCode);
            if (otp is null || otp.Expiration < DateTime.UtcNow)
                return false;

            otps.Remove(otp);

            _cache.SetObject(otpCacheKey, otps);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ValidateOtp: {0}", ex.Message);
            return false;
        }
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
            mime.Subject = "IIngresso: Senha de uso único (OTP)";

            var message = new StringBuilder();
            message.AppendLine("Seguem abaixo sua senha de uso único:<br>");
            message.AppendLine($"Senha: {password}<br>");
            message.AppendLine("Não compartilhe com niguém.");

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
                await client.ConnectAsync(_options.Host, _options.Port, false);
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

    private async Task<bool> SendForgotenAsync(string email, string token)
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
            mime.Subject = "IIngresso: Reset de Senha";

            var message = new StringBuilder();
            message.AppendLine("Segue abaixo o link para redefinição de senha:<br>");
            message.AppendLine($"Link: <a href=\"{baseUrl}/login/reset/{token}\">{baseUrl}/login/reset/{token}</a><br><br>");
            message.AppendLine("Não compartilhe com niguém.");

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
                await client.ConnectAsync(_options.Host, _options.Port, false);
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
    #endregion

    protected class OtpCodeObject
    {
        public long UserId { get; set; }
        public string OtpCode { get; set; }
        public DateTime Expiration { get; set; }
    }
}


