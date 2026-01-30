using Flight.Api.Aircrafts.Models;
using Flight.Api.Aircrafts.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flight.Api.Data.Configurations
{
    // U FlightDbContext ne zelim da sve stavim u OnModelCreating jer bice preveliko i nepregledno, pa svaku tabelu klasu definisem ovde 
    public class AircraftConfiguration : IEntityTypeConfiguration<Aircraft>
    {
        public void Configure(EntityTypeBuilder<Aircraft> builder)
        {
            // Table name definisao u FlightDbContext

            // Soft delete da automatski uzme samo vrste gde IsDeleted = false
            builder.HasQueryFilter(x => !x.IsDeleted); 

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever() // Jer u kodu (Mappers folder) generisem rucno
                                       .HasConversion(id => id.Value,
                                                      dbId => AircraftId.Of(dbId));

            //builder.Property(x => x.RowVersion).IsRowVersion(); ne treba, jer u Entity ima [Timestamp]

            // ValueObject polja of Aircraft namestim da budu kolone u Aircraft tabeli, a ne posebne tabele 
            builder.OwnsOne(x => x.Name,
                            a =>
                            {
                                a.Property(p => p.Value)
                                 .HasColumnName(nameof(Aircraft.Name))
                                 .HasMaxLength(50)
                                 .IsRequired();
                            });

            builder.OwnsOne(x => x.Model,
                            a =>
                            {
                                a.Property(p => p.Value)
                                 .HasColumnName(nameof(Aircraft.Model))
                                 .HasMaxLength(50)
                                 .IsRequired();
                            });

            builder.OwnsOne(x => x.ManufacturingYear,
                            a =>
                            {
                                a.Property(p => p.Value)
                                 .HasColumnName(nameof(Aircraft.ManufacturingYear))
                                 .HasMaxLength(4)
                                 .IsRequired();
                            });
        }
    }
}
