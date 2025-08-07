using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Users;
using MediatR;

namespace Bookify.Application.Bookings.ReserveBooking;

internal sealed class BookingReservedDomainEventHandler(IBookingRepository bookingRepository,
                                                        IUserRepository userRepository,
                                                        IEmailService emailService) : INotificationHandler<BookingReservedDomainEvent>
{
    public async Task Handle(BookingReservedDomainEvent notification, CancellationToken cancellationToken)
    {
        Booking? booking = await bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);

        if (booking is null)
            return;

        User? user = await userRepository.GetByIdAsync(booking.UserId, cancellationToken);

        if (user is null)
            return;

        await emailService.SendAsync(user.Email, subject: "Booking reserved!", body: "You have 10 minutes to confirm this booking");
    }
}