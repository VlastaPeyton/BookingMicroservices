
using BuildingBlocks.Core.Domain;
using BuildingBlocks.UserProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Interceptors
{
    /* Enterprise audit interceptor, ali ne koristim, jer ApplicationDbContextBase koristi UoW gde imam rucni commit, pa SaveChangesAsync nije commit automatski 
     jer SaveChangesInterceptor automatski se pokrece na SaveChangesAsync bez obzira da l SaveChangesAsync radi automatski commit ili ne. 
     Ovu logiku cu da prenesem u ApplicationDbContextBase.CommitTransactionAsync. 
     Ova logika vazi ako NEMAM EventSourcing kao npr u Flight microservice.
    */
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserProvider? _currentUserProvider;
        public AuditInterceptor(ICurrentUserProvider? currentUserProvider)
        {
            _currentUserProvider = currentUserProvider;
        }

        // Audit interceptor zahteva SavingChangesAsync, jer se ona poziva pre SaveChangesAsync i audit ide pre upisa u bazu
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
                                                                              InterceptionResult<int> result, // SaveChangesAsync vraca Task<int>
                                                                              CancellationToken cancellationToken = default)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplyAudit(DbContext? dbContext)
        {
            if (dbContext is null)
                return;

            var now = DateTime.UtcNow;
            var user = _currentUserProvider?.GetCurrentUserId(); 

            foreach (var entry in dbContext.ChangeTracker.Entries<IEntity>())
            {
                if (entry.State == EntityState.Added) // POST
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = user;
                }
                else if (entry.State == EntityState.Modified) // PUT or PATCH
                {
                    // Ne salji update u bazu za ova polja 
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;

                    entry.Entity.LastModifiedAt = now;
                    entry.Entity.LastModifiedBy = user;
                }
                else if (entry.State == EntityState.Deleted) // 
                {   
                    // Soft delete 
                    entry.State = EntityState.Modified;

                    // Ne salji update u bazu za ova polja 
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;

                    entry.Entity.LastModifiedBy = user;
                    entry.Entity.LastModifiedAt = now;
                    entry.Entity.IsDeleted = true;
                }
            }
        }
    }
}
