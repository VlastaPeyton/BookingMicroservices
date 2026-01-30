using Microsoft.EntityFrameworkCore;
using Passenger.Api.Enums;
using Passenger.Api.Models;
using Passenger.Api.ValueObjects;

namespace Passenger.Api.Data.Configurations
{   
    // Da ne bih pisao sve u OnModelCreating zbog guzve
    public class PassengerConfiguration : IEntityTypeConfiguration<Passenger.Api.Models.Passenger>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Passenger.Api.Models.Passenger> builder)
        {
            // Ime tabele u PassengerDbContext

            // Soft delete da automatski uzme samo vrste gde IsDeleted = false
            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).ValueGeneratedNever()   // Jer u kodu (Mappers folder) generisem rucno
                                       .HasConversion(passengerId => passengerId.Value, 
                                                      dbId => PassengerId.Of(dbId));

            //builder.Property(x => x.RowVersion).IsRowVersion(); ne treba, jer u Entity ima [Timestamp]

            // ValueObject polja of Aircraft namestim da budu kolone u Aircraft tabeli, a ne posebne tabele 
            builder.OwnsOne(x => x.Name,
                            a =>
                            {
                                a.Property(p => p.Value)
                                    .HasColumnName(nameof(Passenger.Api.Models.Passenger.Name))
                                    .HasMaxLength(50)
                                    .IsRequired();
                            });

            builder.OwnsOne(x => x.PassportNumber,
                           a =>
                           {
                               a.Property(p => p.Value)
                                   .HasColumnName(nameof(Passenger.Api.Models.Passenger.PassportNumber))
                                   .HasMaxLength(10)
                                   .IsRequired();
                           });

            builder.OwnsOne(x => x.Age,
                            a =>
                            {
                                a.Property(p => p.Value)
                                    .HasColumnName(nameof(Passenger.Api.Models.Passenger.Age))
                                    .HasMaxLength(3)
                                    .IsRequired();
                            });

            // enum kolona da se cuva kao string, a ne int u bazi
            builder.Property(x => x.PassengerType)
                   .HasDefaultValue(PassengerTypeEnum.Unknown)
                   .HasConversion<string>();
        }
    }
}
