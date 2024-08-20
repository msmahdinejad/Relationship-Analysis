using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Category;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices;
using RelationshipAnalysis.Settings.JWT;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RelationshipAnalysis.Integration.Test.Controllers;

public class CategoryControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CategoryControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllNodeCategories_ShouldReturnCorrectList_Whenever()
    {
        // Arrange
        var expectedResult1 = "Person";
        var expectedResult2 = "Account";
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/category/GetAllNodeCategories");
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
            UserRoles = new List<UserRole>() { new UserRole() { Role = new Role() { Name = "Admin" } } }
        };
        var token = new JwtTokenGenerator(mockJwtSettings.Object).GenerateJwtToken(user);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<List<string>>();
        Assert.NotNull(responseData);
        Assert.Contains(expectedResult1, responseData);
        Assert.Contains(expectedResult2, responseData);
    }

    [Fact]
    public async Task GetAllEdgeCategories_ShouldReturnCorrectList_Whenever()
    {
        // Arrange
        var expectedResult1 = "Transaction";
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/category/GetAllEdgeCategories");
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
            UserRoles = new List<UserRole>() { new UserRole() { Role = new Role() { Name = "Admin" } } }
        };
        var token = new JwtTokenGenerator(mockJwtSettings.Object).GenerateJwtToken(user);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<List<string>>();
        Assert.NotNull(responseData);
        Assert.Contains(expectedResult1, responseData);
    }

    [Fact]
    public async Task CreateNodeCategory_ShouldReturnCorrectList_Whenever()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/category/CreateNodeCategory");
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
            UserRoles = new List<UserRole>() { new UserRole() { Role = new Role() { Name = "Admin" } } }
        };
        var token = new JwtTokenGenerator(mockJwtSettings.Object).GenerateJwtToken(user);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new CreateNodeCategoryDto()
        {
            NodeCategoryName = "NotExist"
        };
        request.Content = new StringContent(
            JsonSerializer.Serialize<CreateNodeCategoryDto>(dto),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<MessageDto>();
        Assert.NotNull(responseData);
    }
    
    [Fact]
    public async Task CreateEdgeCategory_ShouldReturnCorrectList_Whenever()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/category/CreateEdgeCategory");
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
            UserRoles = new List<UserRole>() { new UserRole() { Role = new Role() { Name = "Admin" } } }
        };
        var token = new JwtTokenGenerator(mockJwtSettings.Object).GenerateJwtToken(user);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new CreateEdgeCategoryDto()
        {
            EdgeCategoryName = "NotExist"
        };
        request.Content = new StringContent(
            JsonSerializer.Serialize<CreateEdgeCategoryDto>(dto),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<MessageDto>();
        Assert.NotNull(responseData);
    }
}