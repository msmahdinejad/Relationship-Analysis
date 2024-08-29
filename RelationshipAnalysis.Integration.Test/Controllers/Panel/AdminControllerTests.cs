using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.AuthServices;
using RelationshipAnalysis.Settings.JWT;

namespace RelationshipAnalysis.Integration.Test.Controllers.Panel;

public class AdminControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AdminControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private string GenerateAdminJwtToken()
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
            Email = "admin@example.com",
            UserRoles = new List<UserRole> { new UserRole { Role = new Role { Name = "Admin" } } }
        };

        return new JwtTokenGenerator(new Microsoft.Extensions.Options.OptionsWrapper<JwtSettings>(jwtSettings))
            .GenerateJwtToken(user);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUser_WhenAdminIsAuthorized()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/users/1");
        var token = GenerateAdminJwtToken();
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
    public async Task GetUser_ShouldReturnUnauthorized_WhenAdminIsNotAuthorized()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/users/1");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnUsers_WhenAdminIsAuthorized()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/users?page=0&size=10");
        var token = GenerateAdminJwtToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<GetAllUsersDto>();
        Assert.NotNull(responseData);
        Assert.True(responseData.AllUserCount > 0);
        Assert.True(responseData.Users.Count > 0);

    }

    [Fact]
    public async Task GetAllRoles_ShouldReturnRoles_WhenAdminIsAuthorized()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/roles");
        var token = GenerateAdminJwtToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<List<string>>();
        Assert.NotNull(responseData);
        Assert.True(responseData.Count > 0);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnSuccess_WhenAdminIsAuthorized()
    {
        // Arrange
        var token = GenerateAdminJwtToken();

        var createUserDto = new CreateUserDto
        {
            Username = "newuser",
            Password = "Password123!",
            FirstName = "New",
            LastName = "User",
            Email = "newuser@example.com",
            Roles = [ "Admin" ]
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admin/users");
        request.Content = new StringContent(
            JsonSerializer.Serialize(createUserDto),
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
        Assert.Equal(Resources.SucceddfulCreateUser, responseData.Message);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnBadRequest_WhenUsernameIsNotUnique()
    {
        // Arrange
        var token = GenerateAdminJwtToken();

        var createUserDto = new CreateUserDto
        {
            Username = "admin",
            Password = "Password123!",
            FirstName = "Existing",
            LastName = "User",
            Email = "existinguser@example.com",
            Roles = [ "Admin" ]
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admin/users");
        request.Content = new StringContent(
            JsonSerializer.Serialize(createUserDto),
            Encoding.UTF8,
            "application/json"
        );
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseData = await response.Content.ReadFromJsonAsync<MessageDto>();
        Assert.NotNull(responseData);
        Assert.Equal(Resources.UsernameExistsMessage, responseData.Message);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnSuccess_WhenUpdateIsValid()
    {
        // Arrange
        var token = GenerateAdminJwtToken();

        var userUpdateInfoDto = new UserUpdateInfoDto
        {
            Username = "UpdatedName",
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            Email = "updated@example.com"
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/api/admin/users/1");
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
    public async Task UpdateUser_ShouldReturnBadRequest_WhenEmailIsNotValid()
    {
        // Arrange
        var token = GenerateAdminJwtToken();

        var userUpdateInfoDto = new UserUpdateInfoDto
        {
            Username = "UpdatedName",
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            Email = "admin2@example.com"
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/api/admin/users/1");
        request.Content = new StringContent(
            JsonSerializer.Serialize(userUpdateInfoDto),
            Encoding.UTF8,
            "application/json"
        );
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseData = await response.Content.ReadFromJsonAsync<MessageDto>();
        Assert.NotNull(responseData);
        Assert.Equal(Resources.EmailExistsMessage, responseData.Message);
    }

    [Fact]
    public async Task UpdatePassword_ShouldReturnSuccess_WhenPasswordUpdateIsValid()
    {
        // Arrange
        var token = GenerateAdminJwtToken();

        var passwordInfo = new UserPasswordInfoDto
        {
            OldPassword = "validPassword",
            NewPassword = "NewValidPassword1!"
        };

        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/admin/users/1/password");
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
        var responseData = await response.Content.ReadFromJsonAsync<MessageDto>();
        Assert.NotNull(responseData);
        Assert.Equal(Resources.SuccessfulUpdateUserMessage, responseData.Message);
    }

    [Fact]
    public async Task UpdateRoles_ShouldReturnSuccess_WhenUpdateIsValid()
    {
        // Arrange
        var token = GenerateAdminJwtToken();

        var newRoles = new List<string> { "Admin" };

        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/admin/users/1/roles");
        request.Content = new StringContent(
            JsonSerializer.Serialize(newRoles),
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
        Assert.Equal(Resources.SuccessfulUpdateRolesMessage, responseData.Message);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnSuccess_WhenDeleteIsValid()
    {
        // Arrange
        var token = GenerateAdminJwtToken();

        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/admin/users/2");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<MessageDto>();
        Assert.NotNull(responseData);
        Assert.Equal(Resources.SuccessfulDeleteUserMessage, responseData.Message);
    }
}