using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.ValueObjects;
using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Bookings;

public class PricingServiceTests
{
    [Fact]
    public void CalculatePrice_Should_ReturnCorrectTotalPrice()
    {
        // Arrange
        var price = new Money(Amount: 10.0m, Currency.Usd);
        var period = DateRange.Create(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 10));
        Money expectedTotalPrice = price with { Amount = price.Amount * period.LengthInDays };
        Apartment apartment = ApartmentData.Create(price);

        // Act
        PricingDetails pricingDetails = PricingService.CalculatePrice(apartment, period);

        // Assert
        pricingDetails.TotalPrice.Should().Be(expectedTotalPrice);
    }

    [Fact]
    public void CalculatePrice_Should_ReturnCorrectTotalPrice_WhenCleaningFeeIsIncluded()
    {
        // Arrange
        var price = new Money(Amount: 10.0m, Currency.Usd);
        var cleaningFee = new Money(Amount: 99.99m, Currency.Usd);
        var period = DateRange.Create(new DateOnly(year: 2024, month: 1, day: 1), new DateOnly(year: 2024, month: 1, day: 10));
        var expectedTotalPrice = new Money((price.Amount * period.LengthInDays) + cleaningFee.Amount, price.Currency);
        Apartment apartment = ApartmentData.Create(price, cleaningFee);

        // Act
        PricingDetails pricingDetails = PricingService.CalculatePrice(apartment, period);

        // Assert
        pricingDetails.TotalPrice.Should().Be(expectedTotalPrice);
    }
}