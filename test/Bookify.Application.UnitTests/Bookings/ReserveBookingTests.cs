using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Bookings.ReserveBooking;
using Bookify.Application.Exceptions;
using Bookify.Application.UnitTests.Apartments;
using Bookify.Application.UnitTests.Users;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.ValueObjects;
using Bookify.Domain.Users;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Bookify.Application.UnitTests.Bookings;

public class ReserveBookingTests
{
    private static readonly DateTime UtcNow = DateTime.UtcNow;
    private static readonly ReserveBookingCommand Command = new(ApartmentId: Guid.NewGuid(),
                                                                UserId: Guid.NewGuid(),
                                                                new DateOnly(year: 2024, month: 1, day: 1),
                                                                new DateOnly(year: 2024, month: 1, day: 10));

    private readonly ReserveBookingCommandHandler _handler;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IApartmentRepository _apartmentRepositoryMock;
    private readonly IBookingRepository _bookingRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public ReserveBookingTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _apartmentRepositoryMock = Substitute.For<IApartmentRepository>();
        _bookingRepositoryMock = Substitute.For<IBookingRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        var dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
        dateTimeProviderMock.UtcNow.Returns(UtcNow);

        _handler = new ReserveBookingCommandHandler(_userRepositoryMock,
                                                    _apartmentRepositoryMock,
                                                    _bookingRepositoryMock,
                                                    _unitOfWorkMock,
                                                    dateTimeProviderMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserIsNull()
    {
        // Arrange
        _userRepositoryMock
            .GetByIdAsync(Command.UserId, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(returnThis: (User?)null);

        // Act
        Result<Guid> result = await _handler.Handle(request: Command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(UserErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenApartmentIsNull()
    {
        // Arrange
        User user = UserData.Create();

        _userRepositoryMock
            .GetByIdAsync(Command.UserId, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(user);

        _apartmentRepositoryMock
            .GetByIdAsync(Command.ApartmentId, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(returnThis: (Apartment?)null);

        // Act
        Result<Guid> result = await _handler.Handle(request: Command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(ApartmentErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenApartmentIsBooked()
    {
        // Arrange
        User user = UserData.Create();
        Apartment apartment = ApartmentData.Create();
        var duration = DateRange.Create(Command.StartDate, Command.EndDate);

        _userRepositoryMock
            .GetByIdAsync(Command.UserId, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(user);

        _apartmentRepositoryMock
            .GetByIdAsync(Command.ApartmentId, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(apartment);

        _bookingRepositoryMock
            .IsOverlappingAsync(apartment, duration, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(returnThis: true);

        // Act
        Result<Guid> result = await _handler.Handle(request: Command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(expected: BookingErrors.Overlap);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUnitOfWorkThrows()
    {
        // Arrange
        User user = UserData.Create();
        Apartment apartment = ApartmentData.Create();
        var duration = DateRange.Create(Command.StartDate, Command.EndDate);

        _userRepositoryMock
            .GetByIdAsync(Command.UserId, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(user);

        _apartmentRepositoryMock
            .GetByIdAsync(Command.ApartmentId, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(apartment);

        _bookingRepositoryMock
            .IsOverlappingAsync(apartment, duration, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(returnThis: false);

        _unitOfWorkMock
            .SaveChangesAsync()
            .ThrowsAsync(new ConcurrencyException("Concurrency", new Exception()));

        // Act
        Result<Guid> result = await _handler.Handle(request: Command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(expected: BookingErrors.Overlap);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenBookingIsReserved()
    {
        // Arrange
        User user = UserData.Create();
        Apartment apartment = ApartmentData.Create();
        var duration = DateRange.Create(Command.StartDate, Command.EndDate);

        _userRepositoryMock
            .GetByIdAsync(Command.UserId, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(user);

        _apartmentRepositoryMock
            .GetByIdAsync(Command.ApartmentId, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(apartment);

        _bookingRepositoryMock
            .IsOverlappingAsync(apartment, duration, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result<Guid> result = await _handler.Handle(request: Command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_CallRepository_WhenBookingIsReserved()
    {
        // Arrange
        User user = UserData.Create();
        Apartment apartment = ApartmentData.Create();
        var duration = DateRange.Create(Command.StartDate, Command.EndDate);

        _userRepositoryMock
            .GetByIdAsync(Command.UserId, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(user);

        _apartmentRepositoryMock
            .GetByIdAsync(Command.ApartmentId, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(apartment);
        _bookingRepositoryMock
            .IsOverlappingAsync(apartment, duration, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(returnThis: false);

        // Act
        Result<Guid> result = await _handler.Handle(request: Command, CancellationToken.None);

        // Assert
        _bookingRepositoryMock.Received(1).Add(booking: Arg.Is<Booking>(booking => booking.Id == result.Value));
    }
}