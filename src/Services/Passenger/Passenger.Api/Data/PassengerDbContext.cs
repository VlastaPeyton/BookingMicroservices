using System.Reflection;
using BuildingBlocks.EfCore;
using BuildingBlocks.UserProviders;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Passenger.Api.Data
{
    public class PassengerDbContext : ApplicationDbContextBase
    {
        public PassengerDbContext(DbContextOptions<PassengerDbContext> options,
                                  IMediator mediator,
                                  ICurrentUserProvider? currentUserProvider = null)
            : base(options, currentUserProvider, mediator)
        {
            
        }

        public DbSet<Passenger.Api.Models.Passenger> Passengers => Set<Passenger.Api.Models.Passenger>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); // Pokrene sve IEntityTypeConfiguration<T> klase iz Configurations foldera
        }
    }
}
