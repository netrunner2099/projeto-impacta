namespace Credenciamento.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> ListAllAsync();
    Task<User> GetByIdAsync(long id);
    Task<User> GetByEmailAsync(string email);
    Task<User> AddAsync(User entity);
    Task<User> UpdateAsync(User entity);
    Task<bool> DeleteAsync(long id);
    Task<bool> EmailExistsAsync(string email);
}