namespace Credenciamento.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public UserRepository(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<User>> ListAllAsync()
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Users.ToListAsync();
    }

    public async Task<User> GetByIdAsync(long id)
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Users.FindAsync(id);
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> AddAsync(User entity)
    {
        using var db = await _factory.CreateDbContextAsync();
        db.Users.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<User> UpdateAsync(User entity)
    {
        entity.UpdatedAt = DateTime.Now;
        using var db = await _factory.CreateDbContextAsync();
        db.Entry(entity).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        using var db = await _factory.CreateDbContextAsync();
        var user = await db.Users.FindAsync(id);
        if (user == null)
            return false;

        user.UpdatedAt = DateTime.Now;
        user.Status = (byte)UserStatus.Deleted;

        db.Entry(user).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Users.AnyAsync(u => u.Email == email && u.Status != (byte)UserStatus.Deleted);
    }
}