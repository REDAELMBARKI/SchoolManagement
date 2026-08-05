using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Domain.Common.ValueObjects
{
    public class Address : ValueObject
    {
        public string Street { get; }
        public string City { get; }
        public string ZipCode { get; }

        public Address(string street, string city, string zipCode)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new DomainException("Street is required");
            if (string.IsNullOrWhiteSpace(zipCode))
                throw new DomainException("Zip code is required");

            Street = street;
            City = city;
            ZipCode = zipCode;
        }


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return ZipCode;
        }

    }
}
