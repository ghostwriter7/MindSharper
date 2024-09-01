using Microsoft.EntityFrameworkCore.Storage;
using MindSharper.Domain.Repositories;
using MindSharper.Infrastructure.Persistence;

namespace MindSharper.Infrastructure.Repositories;

internal abstract class BaseRepository(MindSharperDatabaseContext context) : IBaseRepository
{
    private IDbContextTransaction? _transaction;
    
    public async Task BeginTransactionAsync()
    {
        _transaction = await context.Database.BeginTransactionAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await context.Database.CommitTransactionAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync()
    {
        await context.Database.RollbackTransactionAsync();
        _transaction = null;
    }
}