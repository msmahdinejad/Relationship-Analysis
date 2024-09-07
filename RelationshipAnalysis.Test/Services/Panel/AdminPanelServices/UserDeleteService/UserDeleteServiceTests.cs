using Moq;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.UserDeleteService.Abstraction;

namespace RelationshipAnalysis.Test.Services.Panel.AdminPanelServices.UserDeleteService;

public class UserDeleteServiceTests
{
    private readonly Mock<IUserDeleter> _mockUserDeleter;
    private readonly Mock<IUserDeleteServiceValidator> _mockValidator;

    private readonly RelationshipAnalysis.Services.Panel.AdminPanelServices.UserDeleteService.UserDeleteService
        _userDeleteService;

    public UserDeleteServiceTests()
    {
        _mockValidator = new Mock<IUserDeleteServiceValidator>();
        _mockUserDeleter = new Mock<IUserDeleter>();
        _userDeleteService =
            new RelationshipAnalysis.Services.Panel.AdminPanelServices.UserDeleteService.UserDeleteService(
                _mockValidator.Object, _mockUserDeleter.Object);
    }

    [Fact]
    public async Task DeleteUser_UserIsInvalid_ReturnsValidationError()
    {
        // Arrange
        var user = new User();
        var validationResponse = new ActionResponse<MessageDto>
        {
            StatusCode = StatusCodeType.NotFound,
            Data = new MessageDto(Resources.UserNotFoundMessage)
        };

        _mockValidator.Setup(v => v.Validate(user))
            .ReturnsAsync(validationResponse);

        // Act
        var result = await _userDeleteService.DeleteUser(user);

        // Assert
        Assert.Equal(StatusCodeType.NotFound, result.StatusCode);
        Assert.Equal(Resources.UserNotFoundMessage, result.Data.Message);
        _mockUserDeleter.Verify(d => d.DeleteUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUser_UserIsValid_DeletesUserAndReturnsSuccess()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com"
        };

        var validationResponse = new ActionResponse<MessageDto>
        {
            StatusCode = StatusCodeType.Success,
            Data = new MessageDto(Resources.SuccessfulDeleteUserMessage)
        };

        _mockValidator.Setup(v => v.Validate(user))
            .ReturnsAsync(validationResponse);

        _mockUserDeleter.Setup(d => d.DeleteUserAsync(user))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userDeleteService.DeleteUser(user);

        // Assert
        Assert.Equal(StatusCodeType.Success, result.StatusCode);
        Assert.Equal(Resources.SuccessfulDeleteUserMessage, result.Data.Message);
        _mockUserDeleter.Verify(d => d.DeleteUserAsync(user), Times.Once);
    }
}