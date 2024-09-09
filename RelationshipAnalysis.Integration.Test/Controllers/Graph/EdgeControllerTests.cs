using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Options;
using Moq;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.AuthServices;
using RelationshipAnalysis.Settings.Authentication;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RelationshipAnalysis.Integration.Test.Controllers.Graph;

public class EdgeControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EdgeControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllEdgeCategories_ShouldReturnCorrectList_Whenever()
    {
        // Arrange
        var expectedResult1 = "Transaction";
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/edge/categories");
        var jwtSettings = new JwtSettings
        {
            Key = "kajbdiuhdqhpjQE89HBSDJIABFCIWSGF89GW3EJFBWEIUBCZNMXCJNLZDKNJKSNJKFBIGW3EASHHDUIASZGCUI",
            ExpireMinutes = 60
        };
        Mock<IOptions<JwtSettings>> mockJwtSettings = new();
        mockJwtSettings.Setup(m => m.Value).Returns(jwtSettings);
        var user = new User
        {
            Username = "Test",
            UserRoles = new List<UserRole> { new() { Role = new Role { Name = "Admin" } } }
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
    public async Task CreateEdgeCategory_ShouldReturnCorrectList_Whenever()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/edge/categories");
        var jwtSettings = new JwtSettings
        {
            Key = "kajbdiuhdqhpjQE89HBSDJIABFCIWSGF89GW3EJFBWEIUBCZNMXCJNLZDKNJKSNJKFBIGW3EASHHDUIASZGCUI",
            ExpireMinutes = 60
        };
        Mock<IOptions<JwtSettings>> mockJwtSettings = new();
        mockJwtSettings.Setup(m => m.Value).Returns(jwtSettings);
        var user = new User
        {
            Username = "Test",
            UserRoles = new List<UserRole> { new() { Role = new Role { Name = "Admin" } } }
        };
        var token = new JwtTokenGenerator(mockJwtSettings.Object).GenerateJwtToken(user);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new CreateEdgeCategoryDto
        {
            EdgeCategoryName = "NotExist"
        };
        request.Content = new StringContent(
            JsonSerializer.Serialize(dto),
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