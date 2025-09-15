using System.Net;
using System.Net.Http.Json;
using Bookify.Api.Controllers.Users;
using Bookify.Api.FunctionalTests.Infrastructure;
using FluentAssertions;

namespace Bookify.Api.FunctionalTests.Users;

public class LoginUserTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    private const string Email = "login@test.com";
    private const string Password = "12345";

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new LogInUserRequest(Email, Password);

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(requestUri: "api/v1/users/login", request);

        // Assert
        response.StatusCode.Should().Be(expected: HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest(Email, FirstName: "first", LastName: "last", Password);
        await HttpClient.PostAsJsonAsync(requestUri: "api/v1/users/register", registerRequest);

        var request = new LogInUserRequest(Email, Password);

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(requestUri: "api/v1/users/login", request);

        // Assert
        response.StatusCode.Should().Be(expected: HttpStatusCode.OK);
    }
}