using BuildingBlocks.Core.Domain;
using Passenger.Api.Enums;
using Passenger.Api.Events.DomainEvents;
using Passenger.Api.ValueObjects;

namespace Passenger.Api.Models
{   
    // Koristim samo DDD, bez ES
    public class Passenger : AggregateRoot<PassengerId>
    {
        public PassportNumber PassportNumber { get; private set; } = default!;
        public Name Name { get; private set; } = default!;
        public PassengerTypeEnum PassengerType { get; private set; } = default!;
        public Age Age { get; private set; } = default!;

        public static Passenger Create(PassengerId passengerId,
                                       Name name,
                                       PassportNumber passportNumber,
                                       Age age,
                                       PassengerTypeEnum passengerType)   
        {
            var passenger = new Passenger
            {
                Id = passengerId,
                Name = name,
                PassportNumber = passportNumber,
                Age = age,
                PassengerType = passengerType
            };

            var domainEvent = new PassengerCreatedDomainEvent(passenger.Id,
                                                              passenger.Name,
                                                              passenger.PassportNumber,
                                                              passenger.Age,
                                                              passenger.PassengerType);

            passenger.AddDomainEvent(domainEvent);

            return passenger;
        }

        public void CompleteRegistrationPassenger(PassengerId id, 
                                                  Name name, 
                                                  PassportNumber passportNumber,
                                                  PassengerTypeEnum passengerType, 
                                                  Age age)
        {
            Id = id;
            Name = name;
            PassportNumber = passportNumber;
            PassengerType = passengerType;
            Age = age;

            var domainEvent = new PassengerRegistrationCompletedDomainEvent(id, 
                                                                            name,
                                                                            passportNumber,
                                                                            passengerType,
                                                                            age);

            AddDomainEvent(domainEvent);
        }
    }
}
