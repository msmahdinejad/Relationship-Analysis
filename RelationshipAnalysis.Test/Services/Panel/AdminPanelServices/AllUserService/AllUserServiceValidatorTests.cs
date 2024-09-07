using FluentAssertions;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService;

namespace RelationshipAnalysis.Test.Services.Panel.AdminPanelServices.AllUserService;

public class AllUserServiceValidatorTests
{
    private readonly AllUserServiceValidator _validator;

    public AllUserServiceValidatorTests()
    {
        _validator = new AllUserServiceValidator();
    }

    [Fact]
    public async Task Validate_ShouldReturnSuccess_WhenUsersListIsNotEmpty()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = 1, Username = "user1", Email = "user1@example.com" },
            new() { Id = 2, Username = "user2", Email = "user2@example.com" }
        };

        // Act
        var result = await _validator.Validate(users);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodeType.Success);
    }

    [Fact]
    public async Task Validate_ShouldReturnNotFound_WhenUsersListIsEmpty()
    {
        // Arrange
        var users = new List<User>();

        // Act
        var result = await _validator.Validate(users);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodeType.NotFound);
    }

    [Fact]
    public async Task Validate_ShouldReturnNotFound_WhenUsersListIsNull()
    {
        // Arrange
        List<User> users = null;

        // Act
        var result = await _validator.Validate(users);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodeType.NotFound);
    }
}