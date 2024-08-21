using System.Text.Json;
using System.Text;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Moq;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices;
using RelationshipAnalysis.Settings.JWT;

namespace RelationshipAnalysis.Integration.Test.Controllers;

public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly JwtSettings _jwtSettings;

    public UserControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _jwtSettings = new JwtSettings
        {
            Key = "kajbdiuhdqhpjQE89HBSDJIABFCIWSGF89GW3EJFBWEIUBCZNMXCJNLZDKNJKSNJKFBIGW3EASHHDUIASZGCUI",
            ExpireMinutes = 60
        };
    }

    [Fact]
    public async Task GetUser_ShouldReturnUser_WhenUserIsAuthorized()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/user");
        var jwtSettings = new JwtSettings
        {
            Key = "kajbdiuhdqhpjQE89HBSDJIABFCIWSGF89GW3EJFBWEIUBCZNMXCJNLZDKNJKSNJKFBIGW3EASHHDUIASZGCUI",
            ExpireMinutes = 60
        };
        Mock<IOptions<JwtSettings>> mockJwtSettings = new();
        mockJwtSettings.Setup(m => m.Value).Returns(jwtSettings);


        var user = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "74b2c5bd3a8de69c8c7c643e8b5c49d6552dc636aeb0995aff6f01a1f661a979",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com"
        };

        var token = new JwtTokenGenerator(mockJwtSettings.Object).GenerateJwtToken(user);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<UserOutputInfoDto>();
        Assert.NotNull(responseData);
        Assert.NotEmpty(responseData.Username);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUnauthorized_WhenUserIsNotAuthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/user");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnSuccess_WhenUpdateIsValid()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "74b2c5bd3a8de69c8c7c643e8b5c49d6552dc636aeb0995aff6f01a1f661a979",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com"
        };

        var userUpdateInfoDto = new UserUpdateInfoDto
        {
            Username = "Updated Name",
            FirstName = "justrandomName",
            LastName = "justrandomName",
            Email = "updated@example.com"
        };

        
        var jwtSettings = new JwtSettings
        {
            Key = "kajbdiuhdqhpjQE89HBSDJIABFCIWSGF89GW3EJFBWEIUBCZNMXCJNLZDKNJKSNJKFBIGW3EASHHDUIASZGCUI",
            ExpireMinutes = 60
        };
        Mock<IOptions<JwtSettings>> mockJwtSettings = new();
        mockJwtSettings.Setup(m => m.Value).Returns(jwtSettings);

        var request = new HttpRequestMessage(HttpMethod.Put, "/api/user");
        request.Content = new StringContent(
            JsonSerializer.Serialize<UserUpdateInfoDto>(userUpdateInfoDto), 
            Encoding.UTF8, 
            "application/json"
        );

        var token = new JwtTokenGenerator(mockJwtSettings.Object).GenerateJwtToken(user);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        
        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<UserOutputInfoDto>();
        Assert.NotNull(responseData);
    }

    [Fact]
    public async Task UpdatePassword_ShouldReturnSuccess_WhenPasswordUpdateIsValid()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "74b2c5bd3a8de69c8c7c643e8b5c49d6552dc636aeb0995aff6f01a1f661a979",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com"
        };

        var passwordInfo = new UserPasswordInfoDto()
        {
            OldPassword = "validPassword",
            NewPassword = "Af3$aaaa"
        };

        
        var jwtSettings = new JwtSettings
        {
            Key = "kajbdiuhdqhpjQE89HBSDJIABFCIWSGF89GW3EJFBWEIUBCZNMXCJNLZDKNJKSNJKFBIGW3EASHHDUIASZGCUI",
            ExpireMinutes = 60
        };
        Mock<IOptions<JwtSettings>> mockJwtSettings = new();
        mockJwtSettings.Setup(m => m.Value).Returns(jwtSettings);

        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/user/password");
        request.Content = new StringContent(
            JsonSerializer.Serialize<UserPasswordInfoDto>(passwordInfo), 
            Encoding.UTF8, 
            "application/json"
        );

        var token = new JwtTokenGenerator(mockJwtSettings.Object).GenerateJwtToken(user);
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
        var user = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "74b2c5bd3a8de69c8c7c643e8b5c49d6552dc636aeb0995aff6f01a1f661a979",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com"
        };

        
        var jwtSettings = new JwtSettings
        {
            Key = "kajbdiuhdqhpjQE89HBSDJIABFCIWSGF89GW3EJFBWEIUBCZNMXCJNLZDKNJKSNJKFBIGW3EASHHDUIASZGCUI",
            ExpireMinutes = 60
        };
        Mock<IOptions<JwtSettings>> mockJwtSettings = new();
        mockJwtSettings.Setup(m => m.Value).Returns(jwtSettings);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/user/logout");

        var token = new JwtTokenGenerator(mockJwtSettings.Object).GenerateJwtToken(user);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        
        // Act
        var response = await _client.SendAsync(request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<UserOutputInfoDto>();
        Assert.NotNull(responseData);
    }
    
    [Fact]
    public async Task GetPermissions_ShouldReturnPermissions_WhenUserIsAuthorized()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/permissions");
        var jwtSettings = new JwtSettings
        {
            Key = "kajbdiuhdqhpjQE89HBSDJIABFCIWSGF89GW3EJFBWEIUBCZNMXCJNLZDKNJKSNJKFBIGW3EASHHDUIASZGCUI",
            ExpireMinutes = 60
        };
        Mock<IOptions<JwtSettings>> mockJwtSettings = new();
        mockJwtSettings.Setup(m => m.Value).Returns(jwtSettings);


        var user = new User()
        {
            Username = "Test",
            UserRoles = new List<UserRole>() { new UserRole() { Role = new Role() { Name = "admin" } } }
        };

        var token = new JwtTokenGenerator(mockJwtSettings.Object).GenerateJwtToken(user);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<PermissionDto>();
        Assert.NotNull(responseData);
        Assert.NotEmpty(responseData.Permissions);
    }

    [Fact]
    public async Task GetPermissions_ShouldReturnUnauthorized_WhenUserIsNotAuthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/user/permissions");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
