namespace Credenciamento.Domain.Interfaces;

public interface IPersonRepository
{
    Task<IEnumerable<Person>> ListAllAsync();
    Task<Person> GetByIdAsync(long id);
    Task<Person> GetByEmailAsync(string email);
    Task<Person> AddAsync(Person entity);
    Task<Person> UpdateAsync(Person entity);
    Task<bool> DeleteAsync(long id);
    Task<bool> DocumentExistsAsync(Person entity);
    Task<bool> PhoneNumberExistsAsync(Person entity);
    Task<bool> EmailExistsAsync(Person entity);
}