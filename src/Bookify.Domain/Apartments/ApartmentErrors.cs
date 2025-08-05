using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;

public static class ApartmentErrors
{
    public static Error NotFound = new(Code: "Apartment.NotFound", Name: "The apartment with the specified identifier was not found");
}