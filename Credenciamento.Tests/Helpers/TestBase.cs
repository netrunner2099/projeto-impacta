using Credenciamento.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System;

namespace Credenciamento.Tests.Helpers;

public abstract class TestBase : IDisposable
{
    protected NonDisposableDbContext Context { get; private set; }
    private readonly string _databaseName;

    protected TestBase()
    {
        // Cada teste terá seu próprio banco de dados in-memory
        _databaseName = Guid.NewGuid().ToString();
        
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .EnableSensitiveDataLogging()
            .Options;

        Context = new NonDisposableDbContext(options);
        
        // Garante que o banco está limpo no início
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        // Limpa o banco ao final do teste
        Context?.Database.EnsureDeleted();
        Context?.ForceDispose();
        GC.SuppressFinalize(this);
    }
}