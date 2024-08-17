using Microsoft.AspNetCore.Http;
using NSubstitute;
using RelationshipAnalysis.Services;

namespace RelationshipAnalysis.Test.Services
{
    public class LogoutServiceTests
    {
        private readonly LogoutService _logoutService = new();
        private readonly HttpResponse _fakeResponse = Substitute.For<HttpResponse>();

        [Fact]
        public void Logout_Should_Delete_Jwt_Cookie()
        {
            // Arrange

            // Act
            _logoutService.Logout(_fakeResponse);

            // Assert
            _fakeResponse.Cookies.Received(1).Delete("jwt");
        }
    }
}