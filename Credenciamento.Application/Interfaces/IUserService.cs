namespace Credenciamento.Application.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Efetua a verificação de login
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <param name="password">Senha cadastrada ou OTP</param>
    /// <returns>Usuário autenticado ou null se a autenticação falhar</returns>
    Task<User> LoginAsync(string email, string password);

    /// <summary>
    /// Cria uma senha OTP e envia para o email.
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <returns>Senha gerada e enviada</returns>
    Task<bool> GenerateOnetTimePasswordAsync(string email);

    /// <summary>
    /// Envia um email para o usuário com um link para redefinir a senha. 
    /// O link contém um token de segurança que é validado no momento da redefinição da senha.
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <returns></returns>
    Task<bool> SendForgotPasswordAsync(string email);

    /// <summary>
    /// Altera a senha do usuário. 
    /// </summary>
    /// <param name="model">Model com os dados de login e senha</param>
    /// <returns></returns>
    Task<bool> ChangePasswordAsync(UserModel model);
}
