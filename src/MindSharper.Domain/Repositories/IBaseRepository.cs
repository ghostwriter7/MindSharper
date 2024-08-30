namespace MindSharper.Domain.Repositories;

public interface IBaseRepository
{
    Task BeginTransactionAsync();
    Task SaveChangesAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}