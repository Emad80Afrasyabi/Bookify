using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users;

public static class UserErrors
{
    public static Error NotFound = new(Code: "User.Found", Name: "The user with the specified identifier was not found");

    public static Error InvalidCredentials = new(Code: "User.InvalidCredentials", Name: "The provided credentials were invalid");
}