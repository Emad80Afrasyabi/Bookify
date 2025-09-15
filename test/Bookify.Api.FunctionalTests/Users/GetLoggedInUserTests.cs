using Bookify.Api.FunctionalTests.Infrastructure;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Bookify.Application.Users.GetLoggedInUser;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Bookify.Api.FunctionalTests.Users;

public class GetLoggedInUserTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task Get_ShouldReturnUnauthorized_WhenAccessTokenIsMissing()
    {
        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(requestUri: "api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(expected: HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_ShouldReturnUser_WhenAccessTokenIsNotMissing()
    {
        // Arrange
        string accessToken = await GetAccessToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

        // Act
        var user = await HttpClient.GetFromJsonAsync<UserResponse>(requestUri: "api/v1/users/me");

        // Assert
        user.Should().NotBeNull();
    }
}
