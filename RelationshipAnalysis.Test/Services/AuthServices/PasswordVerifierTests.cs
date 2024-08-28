using Moq;
using RelationshipAnalysis.Services.AuthServices;
using RelationshipAnalysis.Services.AuthServices.Abstraction;

namespace RelationshipAnalysis.Test.Services.AuthServices;

public class PasswordVerifierTests
{
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly PasswordVerifier _sut;

    public PasswordVerifierTests()
    {
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _sut = new PasswordVerifier(_mockPasswordHasher.Object);
    }

    [Fact]
    public void VerifyPasswordHash_ShouldReturnTrue_WhenPasswordMatchesStoredHash()
    {
        // Arrange
        var password = "1234";
        var storedHash = "03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4";

        _mockPasswordHasher.Setup(p => p.HashPassword(password)).Returns(storedHash);

        // Act
        var result = _sut.VerifyPasswordHash(password, storedHash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPasswordHash_ShouldReturnFalse_WhenPasswordDoesNotMatchStoredHash()
    {
        // Arrange
        var password = "1234";
        var storedHash = "03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4";
        var differentHash = "wrong";

        _mockPasswordHasher.Setup(p => p.HashPassword(password)).Returns(differentHash);

        // Act
        var result = _sut.VerifyPasswordHash(password, storedHash);

        // Assert
        Assert.False(result);
    }
}