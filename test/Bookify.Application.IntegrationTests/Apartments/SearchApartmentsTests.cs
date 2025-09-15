using Bookify.Application.Apartments.SearchApartments;
using Bookify.Application.IntegrationTests.Infrastructure;
using Bookify.Domain.Abstractions;
using FluentAssertions;

namespace Bookify.Application.IntegrationTests.Apartments;

public class SearchApartmentsTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task SearchApartments_ShouldReturnEmptyList_WhenDateRangeInvalid()
    {
        // Arrange
        var query = new SearchApartmentsQuery(new DateOnly(year: 2024, month: 1, day: 10), new DateOnly(year: 2024, month: 1, day: 1));

        // Act
        Result<IReadOnlyList<ApartmentResponse>> result = await Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchApartments_ShouldReturnApartments_WhenDateRangeIsValid()
    {
        // Arrange
        var query = new SearchApartmentsQuery(new DateOnly(year: 2024, month: 1, day: 1), new DateOnly(year: 2024, month: 1, day: 10));

        // Act
        Result<IReadOnlyList<ApartmentResponse>> result = await Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
}
