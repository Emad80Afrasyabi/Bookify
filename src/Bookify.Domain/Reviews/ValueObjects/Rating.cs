using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Reviews.ValueObjects;

public sealed record Rating
{
    public static readonly Error Invalid = new(Code: "Rating.Invalid", Name: "The rating is invalid");

    private Rating(int value) => Value = value;

    public int Value { get; init; }

    public static Result<Rating> Create(int value)
    {
        return value is < 1 or > 5 ? Result.Failure<Rating>(error: Invalid) : new Rating(value);
    }
}