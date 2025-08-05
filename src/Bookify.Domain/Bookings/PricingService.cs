using Bookify.Domain.Apartments;
using Bookify.Domain.Apartments.ValueObjects;
using Bookify.Domain.Bookings.ValueObjects;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;

public static class PricingService
{
    public static PricingDetails CalculatePrice(Apartment apartment, DateRange period)
    {
        Currency currency = apartment.Price.Currency;

        var priceForPeriod = new Money(apartment.Price.Amount * period.LengthInDays, currency);

        decimal percentageUpCharge = apartment.Amenities.Sum(amenity => amenity switch
        {
            Amenity.GardenView or Amenity.MountainView => 0.05m,
            Amenity.AirConditioning => 0.01m,
            Amenity.Parking => 0.01m,
            _ => 0
        });

        Money amenitiesUpCharge = Money.Zero(currency);
        if (percentageUpCharge > 0)
        {
            amenitiesUpCharge = new Money(Amount: priceForPeriod.Amount * percentageUpCharge, currency);
        }

        Money totalPrice = Money.Zero(currency);

        totalPrice += priceForPeriod;

        if (!apartment.CleaningFee.IsZero())
            totalPrice += apartment.CleaningFee;

        totalPrice += amenitiesUpCharge;

        return new PricingDetails(priceForPeriod, apartment.CleaningFee, amenitiesUpCharge, totalPrice);
    }
}