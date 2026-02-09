namespace Credenciamento.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public TicketRepository(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Ticket>> ListAllAsync()
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Tickets
            .Include(t => t.Person)
            .Include(t => t.Event)
            .ToListAsync();
    }

    public async Task<Ticket> GetByIdAsync(long id)
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Tickets
            .Include(t => t.Person)
            .Include(t => t.Event)
            .FirstOrDefaultAsync(t => t.TicketId == id);
    }

    public async Task<IEnumerable<Ticket>> GetByPersonIdAsync(long personId)
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Tickets
            .Include(t => t.Person)
            .Include(t => t.Event)
            .Where(t => t.PersonId == personId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetByEventIdAsync(long eventId)
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Tickets
            .Include(t => t.Person)
            .Include(t => t.Event)
            .Where(t => t.EventId == eventId)
            .ToListAsync();
    }

    public async Task<Ticket> AddAsync(Ticket entity)
    {
        using var db = await _factory.CreateDbContextAsync();
        db.Tickets.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Ticket> UpdateAsync(Ticket entity)
    {
        using var db = await _factory.CreateDbContextAsync();
        entity.UpdatedAt = DateTime.Now;
        db.Entry(entity).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        using var db = await _factory.CreateDbContextAsync();
        var ticket = await db.Tickets.FindAsync(id);
        if (ticket == null)
            return false;

        ticket.UpdatedAt = DateTime.Now;
        ticket.Status = (byte)TicketStatus.Deleted;
        db.Entry(ticket).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return true;
    }
}