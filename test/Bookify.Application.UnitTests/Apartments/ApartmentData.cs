using Bookify.Domain.Apartments;
using Bookify.Domain.Apartments.ValueObjects;
using Bookify.Domain.Shared;

namespace Bookify.Application.UnitTests.Apartments;

internal static class ApartmentData
{
    public static Apartment Create() => new(id: Guid.NewGuid(),
                                            new Name("Test apartment"),
                                            new Description("Test description"),
                                            new Address(Country: "Country", State: "State", ZipCode: "ZipCode", City: "City", "Street"),
                                            price: new Money(Amount: 100.0m, Currency.Usd),
                                            cleaningFee: Money.Zero(),
                                            amenities: []);
}