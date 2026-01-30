using Flight.Api.Aircrafts.Models;
using Flight.Api.Aircrafts.ValueObjects;
using Flight.Api.Airports.Models;
using Flight.Api.Airports.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flight.Api.Data.Configurations
{   
    // Pogledaj AircraftConfiguration 
    public class AirportConfiguration : IEntityTypeConfiguration<Airport>
    {   
        public void Configure(EntityTypeBuilder<Airport> builder)
        {
            // Table name u FlightDbContext definisano

            // Soft delete da automatski uzme samo vrste gde IsDeleted = false
            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever() // Jer u kodu (Mappers folder) generisem rucno
                                       .HasConversion(id => id.Value,
                                                      dbId => AirportId.Of(dbId));

            //builder.Property(x => x.RowVersion).IsRowVersion(); ne treba, jer u Entity ima [Timestamp]

            // ValueObject polja of Airport namestim da budu kolone u Aircraft tabeli, a ne posebne tabele 
            builder.OwnsOne(x => x.Name,
                            a =>
                            {
                               a.Property(p => p.Value)
                                .HasColumnName(nameof(Airport.Name))
                                .HasMaxLength(50)
                                .IsRequired();
                            });

            builder.OwnsOne(x => x.Address,
                            a =>
                            {
                                a.Property(p => p.Value)
                                 .HasColumnName(nameof(Airport.Address))
                                 .HasMaxLength(50)
                                 .IsRequired();
                            });

            builder.OwnsOne(x => x.Code,
                            a =>
                            {
                                a.Property(p => p.Value)
                                  .HasColumnName(nameof(Airport.Code))
                                  .HasMaxLength(50)
                                  .IsRequired();
                            });
        }
    }
}
