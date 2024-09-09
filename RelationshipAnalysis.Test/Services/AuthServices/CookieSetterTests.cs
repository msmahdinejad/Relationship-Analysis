using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using RelationshipAnalysis.Services.AuthServices;
using RelationshipAnalysis.Settings.Authentication;

namespace RelationshipAnalysis.Test.Services.AuthServices;

public class CookieSetterTests
{
    private readonly JwtSettings _jwtSettings;
    private readonly Mock<IResponseCookies> _mockCookies;
    private readonly Mock<HttpResponse> _mockHttpResponse;
    private readonly CookieSetter _sut;

    public CookieSetterTests()
    {
        _jwtSettings = new JwtSettings
        {
            CookieName = "TestCookie",
            ExpireMinutes = 30
        };

        var mockJwtSettings = new Mock<IOptions<JwtSettings>>();
        mockJwtSettings.Setup(m => m.Value).Returns(_jwtSettings);

        _sut = new CookieSetter(mockJwtSettings.Object);

        _mockCookies = new Mock<IResponseCookies>();

        _mockHttpResponse = new Mock<HttpResponse>();
        _mockHttpResponse.SetupGet(r => r.Cookies).Returns(_mockCookies.Object);
    }

    [Fact]
    public void SetCookie_ShouldAppendCookieWithExpectedValues_Whenever()
    {
        // Arrange
        var token = "testToken";
        var expiration = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes);

        // Act
        _sut.SetCookie(_mockHttpResponse.Object, token);

        // Assert
        _mockCookies.Verify(c => c.Append(
            _jwtSettings.CookieName,
            token,
            It.Is<CookieOptions>(options =>
                options.HttpOnly == true &&
                options.SameSite == SameSiteMode.Strict &&
                options.Secure == true &&
                options.Expires.HasValue &&
                Math.Abs((options.Expires.Value - expiration).TotalSeconds) < 1
            )
        ), Times.Once);
    }
}