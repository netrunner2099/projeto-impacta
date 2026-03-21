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
}
