using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Bookings.ValueObjects;
using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using Bookify.Domain.UnitTests.Infrastructure;
using Bookify.Domain.UnitTests.Users;
using Bookify.Domain.Users;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Bookings;

public class BookingTests : BaseTest
{
    [Fact]
    public void Reserve_Should_RaiseBookingReservedDomainEvent()
    {
        // Arrange
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        var price = new Money(Amount: 10.0m, Currency.Usd);
        var duration = DateRange.Create(new DateOnly(year: 2024, month: 1, day: 1), new DateOnly(year: 2024, month: 1, day: 10));
        Apartment apartment = ApartmentData.Create(price);

        // Act
        Booking booking = Booking.Reserve(apartment, user.Id, duration, DateTime.UtcNow);

        // Assert
        var bookingReservedDomainEvent = AssertDomainEventWasPublished<BookingReservedDomainEvent>(booking);

        bookingReservedDomainEvent.BookingId.Should().Be(booking.Id);
    }
}
