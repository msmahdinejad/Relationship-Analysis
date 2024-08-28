using RelationshipAnalysis.Services.AuthServices;

namespace RelationshipAnalysis.Test.Services.AuthServices;

public class CustomPasswordHasherTests
{
    private readonly CustomPasswordHasher _sut;

    public CustomPasswordHasherTests()
    {
        _sut = new CustomPasswordHasher();
    }

    [Fact]
    public void HashPassword_ShouldReturnExpectedHash_WhenValueIs1234()
    {
        // Arrange
        var password = "1234";
        var expectedHash = "03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4";

        // Act
        var hash = _sut.HashPassword(password);

        // Assert
        Assert.Equal(hash, expectedHash);
    }
}