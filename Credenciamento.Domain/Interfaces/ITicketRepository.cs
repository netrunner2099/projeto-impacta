namespace Credenciamento.Domain.Interfaces;

public interface ITicketRepository
{
    Task<IEnumerable<Ticket>> ListAllAsync();
    Task<Ticket> GetByIdAsync(long id);
    Task<IEnumerable<Ticket>> GetByPersonIdAsync(long personId);
    Task<IEnumerable<Ticket>> GetByEventIdAsync(long eventId);
    Task<Ticket> AddAsync(Ticket entity);
    Task<Ticket> UpdateAsync(Ticket entity);
    Task<bool> DeleteAsync(long id);
}