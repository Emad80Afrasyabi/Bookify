using FluentValidation;

namespace Bookify.Application.Bookings.ReserveBooking;

public class ReserveBookingCommandValidator : AbstractValidator<ReserveBookingCommand>
{
    public ReserveBookingCommandValidator()
    {
        RuleFor(reserveBookingCommand => reserveBookingCommand.UserId).NotEmpty();

        RuleFor(reserveBookingCommand => reserveBookingCommand.ApartmentId).NotEmpty();

        RuleFor(reserveBookingCommand => reserveBookingCommand.StartDate).LessThan(reserveBookingCommand => reserveBookingCommand.EndDate);
    }
}