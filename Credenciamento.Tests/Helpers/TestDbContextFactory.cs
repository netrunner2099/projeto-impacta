using Credenciamento.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Credenciamento.Tests.Helpers;

public class TestDbContextFactory : IDbContextFactory<ApplicationDbContext>
{
    private readonly ApplicationDbContext _context;

    public TestDbContextFactory(ApplicationDbContext context)
    {
        _context = context;
    }

    public ApplicationDbContext CreateDbContext()
    {
        return _context;
    }

    public Task<ApplicationDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_context);
    }
}