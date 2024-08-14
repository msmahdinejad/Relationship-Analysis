using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Test.Services;

public class LoginServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly LoginService _sut;
    private readonly Mock<ICookieSetter> _mockCookieSetter;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
    private readonly Mock<IPasswordVerifier> _mockPasswordVerifier;
    private readonly Mock<HttpResponse> _mockHttpResponse;

    public LoginServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        SeedDatabase();

        _mockCookieSetter = new Mock<ICookieSetter>();
        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        _mockPasswordVerifier = new Mock<IPasswordVerifier>();

        Mock<IResponseCookies> mockResponseCookies = new();
        _mockHttpResponse = new Mock<HttpResponse>();
        _mockHttpResponse.SetupGet(r => r.Cookies).Returns(mockResponseCookies.Object);

        _sut = new LoginService(
            _context,
            _mockCookieSetter.Object,
            _mockJwtTokenGenerator.Object,
            _mockPasswordVerifier.Object
        );
    }

    private void SeedDatabase()
    {
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = "correctPasswordHash",
            FirstName = "Test",
            LastName = "User",
            Email = "testuser@example.com"
        };
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "correctPassword" };
        var token = "validToken";

        _mockPasswordVerifier.Setup(p => p.VerifyPasswordHash(loginDto.Password, It.IsAny<string>()))
            .Returns(true);
        _mockJwtTokenGenerator.Setup(j => j.GenerateJwtToken(It.IsAny<User>()))
            .Returns(token);

        // Act
        var result = await _sut.LoginAsync(loginDto, _mockHttpResponse.Object);

        // Assert
        Assert.Equal(StatusCodeType.Success, result.StatusCode);
        Assert.Equal("Login was successful!", result.Data.Message);
        _mockCookieSetter.Verify(c => c.SetCookie(_mockHttpResponse.Object, token), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "nonexistentuser", Password = "password" };

        // Act
        var result = await _sut.LoginAsync(loginDto, _mockHttpResponse.Object);

        // Assert
        Assert.Equal(StatusCodeType.Unauthorized, result.StatusCode);
        Assert.Equal("Login failed!", result.Data.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenPasswordIsIncorrect()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "wrongPassword" };
        _mockPasswordVerifier.Setup(p => p.VerifyPasswordHash(loginDto.Password, It.IsAny<string>()))
            .Returns(false);

        // Act
        var result = await _sut.LoginAsync(loginDto, _mockHttpResponse.Object);

        // Assert
        Assert.Equal(StatusCodeType.Unauthorized, result.StatusCode);
        Assert.Equal("Login failed!", result.Data.Message);
    }
}