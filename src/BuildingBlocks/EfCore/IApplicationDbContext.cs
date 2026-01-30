using BuildingBlocks.Core.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.EfCore
{   
    // UnitOfWork 
    public interface IApplicationDbContext
    {   
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        IReadOnlyList<IDomainEvent> GetAndClearDomainEvents();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
        Task ExecuteTransactionalAsync(CancellationToken cancellationToken);
        IExecutionStrategy CreateExecutionStrategy(); 
    }
}
