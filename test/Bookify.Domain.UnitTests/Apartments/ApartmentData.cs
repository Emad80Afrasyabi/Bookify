using Bookify.Domain.Apartments;
using Bookify.Domain.Apartments.ValueObjects;
using Bookify.Domain.Shared;

namespace Bookify.Domain.UnitTests.Apartments;

internal static class ApartmentData
{
    public static Apartment Create(Money price, Money? cleaningFee = null) => new(id: Guid.NewGuid(),
                                                                                  new Name("Test apartment"),
                                                                                  new Description("Test description"),
                                                                                  new Address(Country: "Country", State: "State", ZipCode: "ZipCode", City: "City", "Street"),
                                                                                  price,
                                                                                  cleaningFee ?? Money.Zero(),
                                                                                  amenities: []);
}