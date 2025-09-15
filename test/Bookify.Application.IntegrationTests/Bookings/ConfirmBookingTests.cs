using Bookify.Application.Bookings.ConfirmBooking;
using Bookify.Application.IntegrationTests.Infrastructure;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using FluentAssertions;

namespace Bookify.Application.IntegrationTests.Bookings;

public class ConfirmBookingTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    private static readonly Guid BookingId = Guid.NewGuid();

    [Fact]
    public async Task ConfirmBooking_ShouldReturnFailure_WhenBookingIsNotFound()
    {
        // Arrange
        var command = new ConfirmBookingCommand(BookingId);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.Should().Be(expected: BookingErrors.NotFound);
    }
}