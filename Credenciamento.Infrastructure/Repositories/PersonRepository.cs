namespace Credenciamento.Infrastructure.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public PersonRepository(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Person>> ListAllAsync()
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Persons.ToListAsync();
    }

    public async Task<Person> GetByIdAsync(long id)
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Persons.FindAsync(id);
    }

    public async Task<Person> GetByEmailAsync(string email)
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Persons.FirstOrDefaultAsync(q => q.Email == email);
    }

    public async Task<Person> AddAsync(Person entity)
    {
        using var db = await _factory.CreateDbContextAsync();
        db.Persons.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Person> UpdateAsync(Person entity)
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
        var entity = await db.Persons.FindAsync(id);
        if (entity == null)
            return false;

        entity.Status = (byte)PersonStatus.Deleted;
        entity.UpdatedAt = DateTime.Now;
        db.Entry(entity).State = EntityState.Modified;
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DocumentExistsAsync(Person entity)
    {
        var db = await _factory.CreateDbContextAsync();
        return await db.Persons.AnyAsync(p => p.Document == entity.Document && p.PersonId != entity.PersonId && p.Status != (byte)PersonStatus.Deleted);
    }

    public async Task<bool> PhoneNumberExistsAsync(Person entity)
    {
        var db = await _factory.CreateDbContextAsync();
        return await db.Persons.AnyAsync(p => p.Phone == entity.Phone && p.PersonId != entity.PersonId && p.Status != (byte)PersonStatus.Deleted);
    }

    public async Task<bool> EmailExistsAsync(Person entity)
    {
        var db = await _factory.CreateDbContextAsync();
        return await db.Persons.AnyAsync(p => p.Email == entity.Email && p.PersonId != entity.PersonId && p.Status != (byte)PersonStatus.Deleted);
    }
}