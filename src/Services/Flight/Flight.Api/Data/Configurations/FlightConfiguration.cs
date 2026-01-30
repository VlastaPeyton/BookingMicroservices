
using Flight.Api.Aircrafts.Models;
using Flight.Api.Airports.Models;
using Flight.Api.Flights.Enums;
using Flight.Api.Flights.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Flight.Api.Data.Configurations
{   
    // Pogledaj AircraftConfiguration 
    public class FlightConfiguration : IEntityTypeConfiguration<Flight.Api.Flights.Models.Flight>
    {   
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Flight.Api.Flights.Models.Flight> builder)
        {
            // Table name u FlightDbContext definisano

            // Soft delete da automatski uzme samo vrste gde IsDeleted = false
            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever() // Jer u kodu (Mappers folder) generisem rucno
                                       .HasConversion(id => id.Value,
                                                      dbId => FlightId.Of(dbId));

            //builder.Property(x => x.RowVersion).IsRowVersion(); ne treba, jer u Entity ima [Timestamp]

            // ValueObject polja of Flight namestim da budu kolone u Aircraft tabeli, a ne posebne tabele 
            builder.OwnsOne(x => x.FlightNumber,
                            a =>
                            {
                                a.Property(p => p.Value)
                                 .HasColumnName(nameof(Flight.Api.Flights.Models.Flight.FlightNumber))
                                 .HasMaxLength(50)
                                 .IsRequired();
                            });

            builder.OwnsOne(x => x.DurationMinutes,
                            a =>
                            {
                                a.Property(p => p.Value)
                                    .HasColumnName(nameof(Flight.Api.Flights.Models.Flight.DurationMinutes))
                                    .HasMaxLength(50)
                                    .IsRequired();
                            });

            builder.OwnsOne(x => x.Price,
                            a =>
                            {
                                a.Property(p => p.Value)
                                    .HasColumnName(nameof(Flight.Api.Flights.Models.Flight.Price))
                                    .HasMaxLength(10)
                                    .IsRequired();
                            });

            builder.OwnsOne(x => x.ArriveDate,
                            a =>
                            {
                                a.Property(p => p.Value)
                                    .HasColumnName(nameof(Flight.Api.Flights.Models.Flight.ArriveDate))
                                    .IsRequired();
                            });

            builder.OwnsOne(x => x.DepartureDate,
                            a =>
                            {
                                a.Property(p => p.Value)
                                    .HasColumnName(nameof(Flight.Api.Flights.Models.Flight.DepartureDate))
                                    .IsRequired();
                            });

            builder.OwnsOne(x => x.FlightDate,
                            a =>
                            {
                                a.Property(p => p.Value)
                                    .HasColumnName(nameof(Flight.Api.Flights.Models.Flight.FlightDate))
                                    .IsRequired();
                            });

            // enum kolona da se cuva kao string, a ne int u bazi
            builder.Property(x => x.Status)
                   .HasDefaultValue(FlightStatusEnum.Unknown)
                   .HasConversion<string>();

            // PK-FK relations. U DDD nema Navigacione atribute, da se ne mesaju domeni.
            builder.HasOne<Aircraft>()
                   .WithMany()
                   .HasForeignKey(p => p.AircraftId)
                   .IsRequired();

            builder.HasOne<Airport>()
                   .WithMany()
                   .HasForeignKey(p => p.ArriveAirportId)
                   .IsRequired();

            builder.HasOne<Airport>()
                   .WithMany()
                   .HasForeignKey(p => p.DepartureAirportId)
                   .IsRequired();
        }
    }
}
