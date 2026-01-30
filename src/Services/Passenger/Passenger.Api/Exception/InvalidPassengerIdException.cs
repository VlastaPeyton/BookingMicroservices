using BuildingBlocks.Exceptions;

namespace Passenger.Api.Exception
{
    public class InvalidPassengerIdException : DomainException
    {
        public InvalidPassengerIdException(Guid passengerId)
            : base($"PassengerId: '{passengerId}' is invalid.")
        {
        }
    }
}
