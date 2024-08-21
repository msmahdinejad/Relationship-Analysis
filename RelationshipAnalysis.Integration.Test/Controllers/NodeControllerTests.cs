using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using NSubstitute;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Category;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices;
using RelationshipAnalysis.Settings.JWT;

namespace RelationshipAnalysis.Integration.Test.Controllers;

public class NodeControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public NodeControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllNodeCategories_ShouldReturnCorrectList_Whenever()
    {
        // Arrange
        var expectedResult1 = "Person";
        var expectedResult2 = "Account";
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/node/categories");
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
    public async Task CreateNodeCategory_ShouldReturnCorrectList_Whenever()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/node/categories");
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
    public async Task UploadNode_ShouldReturnSuccess_WhenDtoIsValid()
    {
        // Arrange
        var csvContent = @"""AccountID"",""CardID"",""IBAN""
""6534454617"",""6104335000000190"",""IR120778801496000000198""
""4000000028"",""6037699000000020"",""IR033880987114000000028""
";
        var mockFile = CreateFileMock(csvContent);

        var fileContent = new StreamContent(mockFile.OpenReadStream());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
        
        var formDataContent = new MultipartFormDataContent();
        formDataContent.Add(new StringContent("Account"), "NodeCategoryName");
        formDataContent.Add(new StringContent("AccountID"), "UniqueAttributeHeaderName");
        formDataContent.Add(fileContent, "file", mockFile.FileName);

        var request = new HttpRequestMessage(HttpMethod.Post, "api/node");
        
        request.Content = formDataContent;
        
        var jwtSettings = new JwtSettings
        {
            Key = "kajbdiuhdqhpjQE89HBSDJIABFCIWSGF89GW3EJFBWEIUBCZNMXCJNLZDKNJKSNJKFBIGW3EASHHDUIASZGCUI",
            ExpireMinutes = 60
        };
        
        Mock<IOptions<JwtSettings>> jwtSettingsMock = new();
        jwtSettingsMock.Setup(m => m.Value).Returns(jwtSettings);
        
        var user = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "74b2c5bd3a8de69c8c7c643e8b5c49d6552dc636aeb0995aff6f01a1f661a979",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com",
            UserRoles = new List<UserRole>() { new UserRole() { Role = new Role() { Name = "admin" } } }

        };

        var token = new JwtTokenGenerator(jwtSettingsMock.Object).GenerateJwtToken(user);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        
        // Act
        var response = await _client.SendAsync(request);
        
        // Assert
        Assert.Equal(200, (int)response.StatusCode);
        Assert.Equal(Resources.SuccessfulNodeAdditionMessage, response.Content.ReadFromJsonAsync<MessageDto>().Result.Message);
    }

    private IFormFile CreateFileMock(string csvContent)
    {
        var csvFileName = "test.csv";
        var fileMock = Substitute.For<IFormFile>();
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(csvContent);
        writer.Flush();
        stream.Position = 0;

        fileMock.OpenReadStream().Returns(stream);
        fileMock.FileName.Returns(csvFileName);
        fileMock.Length.Returns(stream.Length);
        return fileMock;
    }
}