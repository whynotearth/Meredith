using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Tenant
{
    public class AddressResult
    {
        public string? Street { get; }

        public string? ApartmentNumber { get; }

        public string? City { get; }

        public string? ZipCode { get; }

        public string? State { get; }

        public AddressResult(Address? address)
        {
            if (address is null)
            {
                return;
            }

            Street = address.Street;
            ApartmentNumber = address.ApartmentNumber;
            City = address.City;
            ZipCode = address.ZipCode;
            State = address.State;
        }
    }
}