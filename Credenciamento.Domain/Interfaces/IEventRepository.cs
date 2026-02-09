namespace Credenciamento.Domain.Interfaces;

public interface IEventRepository
{
    Task<IEnumerable<Event>> ListAllAsync();
    Task<Event> GetByIdAsync(long id);
    Task<Event> AddAsync(Event entity);
    Task<Event> UpdateAsync(Event entity);
    Task<bool> DeleteAsync(long id);
}