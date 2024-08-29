using Microsoft.AspNetCore.Http;
using NSubstitute;
using RelationshipAnalysis.Services.Panel.UserPanelServices.LogoutService;

namespace RelationshipAnalysis.Test.Services.Panel.UserPanelServices.LogOutService;

public class LogoutServiceTests
{
    private readonly HttpResponse _fakeResponse = Substitute.For<HttpResponse>();
    private readonly LogoutService _logoutService = new();

    [Fact]
    public void Logout_Should_Delete_Jwt_Cookie()
    {
        // Arrange

        // Act
        _logoutService.Logout(_fakeResponse);

        // Assert
        _fakeResponse.Cookies.Received().Delete("jwt");
    }
}