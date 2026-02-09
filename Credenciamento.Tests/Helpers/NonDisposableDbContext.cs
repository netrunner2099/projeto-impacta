using Credenciamento.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Credenciamento.Tests.Helpers;

public class NonDisposableDbContext : ApplicationDbContext
{
    public NonDisposableDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    // Sobrescreve o Dispose para não fazer nada nos testes
    public override void Dispose()
    {
        // Não descarta o contexto
    }

    public override ValueTask DisposeAsync()
    {
        // Não descarta o contexto
        return ValueTask.CompletedTask;
    }

    // Método para forçar dispose quando realmente necessário
    public void ForceDispose()
    {
        base.Dispose();
    }
}