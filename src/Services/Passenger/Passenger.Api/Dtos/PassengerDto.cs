using Passenger.Api.Enums;

namespace Passenger.Api.Dtos
{
    public record PassengerDto(Guid PassengerId,
                               string PassengerName,
                               string PassportNumber,
                               PassengerTypeEnum PassengerType,  // U Mongo mi je lakse da sve string bude nego enum pa on da prevodi automatski iz tipa u tip
                               int Age);
}
