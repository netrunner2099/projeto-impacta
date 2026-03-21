namespace Credenciamento.Domain.Interfaces;

public interface ITicketRepository
{
    Task<IEnumerable<Ticket>> ListAllAsync();
    Task<Ticket> GetByIdAsync(long id);
    Task<Ticket> GetByTransactionAsync(string transaction);
    Task<IEnumerable<Ticket>> ListByPersonIdAsync(long personId);
    Task<IEnumerable<Ticket>> ListByEventIdAsync(long eventId);
    Task<Ticket> AddAsync(Ticket entity);
    Task<Ticket> UpdateAsync(Ticket entity);
    Task<bool> DeleteAsync(long id);
}