using System.Net;
using System.Net.Http.Json;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Auth;

namespace RelationshipAnalysis.Integration.Test.Controllers.Auth;

public class HomeControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HomeControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var loginModel = new LoginDto
        {
            Username = "admin",
            Password = "validPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<MessageDto>();
        Assert.NotNull(responseData);
        Assert.Equal("Login was successful!", responseData.Message);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var loginModel = new LoginDto
        {
            Username = "admin",
            Password = "invalidPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenNoCredentialsAreProvided()
    {
        // Act
        var loginModel = new LoginDto { Username = string.Empty, Password = string.Empty };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}