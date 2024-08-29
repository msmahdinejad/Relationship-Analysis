using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService;

namespace RelationshipAnalysis.Test.Services.Panel.UserPanelServices.UserInfoService;

public class UserInfoServiceValidatorTests
{
    private readonly UserInfoServiceValidator _sut;

    public UserInfoServiceValidatorTests()
    {
        _sut = new UserInfoServiceValidator();
    }

    [Fact]
    public async Task Validate_UserIsNull_ShouldReturnNotFound()
    {
        // Arrange
        User nullUser = null;

        // Act
        var result = await _sut.Validate(nullUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodeType.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Validate_UserIsNotNull_ShouldReturnSuccess()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser"
        };

        // Act
        var result = await _sut.Validate(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodeType.Success, result.StatusCode);
    }
}