using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.AuthServices;
using RelationshipAnalysis.Settings.JWT;

namespace RelationshipAnalysis.Integration.Test.Controllers.Panel;

public class UserControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UserControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private string GenerateJwtToken()
    {
        var jwtSettings = new JwtSettings
        {
            Key = "kajbdiuhdqhpjQE89HBSDJIABFCIWSGF89GW3EJFBWEIUBCZNMXCJNLZDKNJKSNJKFBIGW3EASHHDUIASZGCUI",
            ExpireMinutes = 60
        };

        var user = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "74b2c5bd3a8de69c8c7c643e8b5c49d6552dc636aeb0995aff6f01a1f661a979",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com"
        };

        return new JwtTokenGenerator(new Microsoft.Extensions.Options.OptionsWrapper<JwtSettings>(jwtSettings)).GenerateJwtToken(user);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUser_WhenUserIsAuthorized()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/user");
        var token = GenerateJwtToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<UserOutputInfoDto>();
        Assert.NotNull(responseData);
        Assert.Equal("admin", responseData.Username);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUnauthorized_WhenUserIsNotAuthorized()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/user");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnSuccess_WhenUpdateIsValid()
    {
        // Arrange
        var token = GenerateJwtToken();

        var userUpdateInfoDto = new UserUpdateInfoDto
        {
            Username = "UpdatedName",
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            Email = "updated@example.com"
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/api/user");
        request.Content = new StringContent(
            JsonSerializer.Serialize(userUpdateInfoDto),
            Encoding.UTF8,
            "application/json"
        );
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<MessageDto>();
        Assert.NotNull(responseData);
        Assert.Equal(Resources.SuccessfulUpdateUserMessage, responseData.Message);
    }

    [Fact]
    public async Task UpdatePassword_ShouldReturnSuccess_WhenPasswordUpdateIsValid()
    {
        // Arrange
        var token = GenerateJwtToken();

        var passwordInfo = new UserPasswordInfoDto
        {
            OldPassword = "validPassword",
            NewPassword = "NewValidPassword1!"
        };

        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/user/password");
        request.Content = new StringContent(
            JsonSerializer.Serialize(passwordInfo),
            Encoding.UTF8,
            "application/json"
        );
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<UserOutputInfoDto>();
        Assert.NotNull(responseData);
    }

    [Fact]
    public async Task Logout_ShouldReturnOk_OnSuccessfulLogout()
    {
        // Arrange
        var token = GenerateJwtToken();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/user/logout");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<MessageDto>();
        Assert.NotNull(responseData);
        Assert.Equal(Resources.SuccessfulLogoutMessage, responseData.Message);
    }

    [Fact]
    public async Task GetPermissions_ShouldReturnPermissions_WhenUserIsAuthorized()
    {
        // Arrange
        var token = GenerateJwtToken();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/permissions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<PermissionDto>();
        Assert.NotNull(responseData);
        Assert.Contains("AdminPermissions", responseData.Permissions);
    }

    [Fact]
    public async Task GetPermissions_ShouldReturnUnauthorized_WhenUserIsNotAuthorized()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/permissions");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}