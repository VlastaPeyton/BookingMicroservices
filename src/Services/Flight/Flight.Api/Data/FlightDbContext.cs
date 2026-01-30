using System.Reflection;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.EfCore;
using BuildingBlocks.UserProviders;
using Flight.Api.Aircrafts.Models;
using Flight.Api.Airports.Models;
using Flight.Api.Seats.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flight.Api.Data
{
    public class FlightDbContext : ApplicationDbContextBase
    {   
        public FlightDbContext(DbContextOptions<FlightDbContext> options,
                               IMediator mediator,
                               ICurrentUserProvider? currentUserProvider = null)
            : base(options, currentUserProvider, mediator)
        {
            
        }

        public DbSet<Flights.Models.Flight> Flights => Set<Flights.Models.Flight>();
        public DbSet<Airport> Airports => Set<Airport>();
        public DbSet<Aircraft> Aircrafts => Set<Aircraft>();
        public DbSet<Seat> Seats => Set<Seat>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); // Pokrene sve IEntityTypeConfiguration<T> klase iz Configurations foldera
        }
    }
}
