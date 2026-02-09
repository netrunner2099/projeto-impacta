namespace Credenciamento.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public EventRepository(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Event>> ListAllAsync()
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Events.ToListAsync();
    }

    public async Task<IEnumerable<Event>> ListFutureAsync()
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Events.Where(q => q.Begin.Date >= DateTime.Now.Date).ToListAsync();
    }

    public async Task<Event> GetByIdAsync(long id)
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Events.FindAsync(id);
    }

    public async Task<Event> AddAsync(Event entity)
    {
        using var db = await _factory.CreateDbContextAsync();
        db.Events.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Event> UpdateAsync(Event entity)
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
        var entity = await db.Events.FindAsync(id);
        if (entity == null)
            return false;

        entity.UpdatedAt = DateTime.Now;
        entity.Status = (byte)EventStatus.Deleted;
        db.Entry(entity).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return true;
    }

}