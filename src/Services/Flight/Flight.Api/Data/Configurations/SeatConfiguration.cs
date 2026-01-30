using Flight.Api.Flights.ValueObjects;
using Flight.Api.Seats.Enums;
using Flight.Api.Seats.Models;
using Flight.Api.Seats.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flight.Api.Data.Configurations
{
    public class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            // Table name u FlightDbContext definisano

            // Soft delete da automatski uzme samo vrste gde IsDeleted = false
            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever() // Jer u kodu (Mappers folder) generisem rucno
                                       .HasConversion(id => id.Value,
                                                      dbId => SeatId.Of(dbId));

            //builder.Property(x => x.RowVersion).IsRowVersion(); ne treba, jer u Entity ima [Timestamp]

            // ValueObject polja of Seat namestim da budu kolone u Aircraft tabeli, a ne posebne tabele 
            builder.OwnsOne(x => x.SeatNumber,
                            a =>
                            {
                                a.Property(p => p.Value)
                                    .HasColumnName(nameof(Seat.SeatNumber))
                                    .HasMaxLength(50)
                                    .IsRequired();
                            });

            // enum kolona da se cuva kao string, a ne int u bazi
            builder.Property(x => x.Type)
                  .HasDefaultValue(SeatTypeEnum.Unknown)
                  .HasConversion<string>();

            builder.Property(x => x.Class)
                  .HasDefaultValue(SeatClassEnum.Unknown)
                  .HasConversion<string>();

            // PK-FK relations. U DDD nema Navigacione atribute, da se ne mesaju domeni.
            builder.HasOne<Flight.Api.Flights.Models.Flight>()
                   .WithMany()
                   .HasForeignKey(p => p.FlightId)
                   .IsRequired();
        }
    }
}
