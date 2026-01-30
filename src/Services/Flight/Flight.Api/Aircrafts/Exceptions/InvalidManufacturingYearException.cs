using BuildingBlocks.Exceptions;

namespace Flight.Api.Aircrafts.Exceptions
{
    public class InvalidManufacturingYearException : DomainException
    {
        public InvalidManufacturingYearException() : base("ManufacturingYear must be greater than 1900")
        {
        }
    }
}
