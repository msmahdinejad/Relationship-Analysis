using Moq;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.UserDeleteService;

namespace RelationshipAnalysis.Test.Services.Panel.AdminPanelServices.UserDeleteService;

public class UserDeleteServiceValidatorTests
{
    private readonly Mock<IMessageResponseCreator> _mockMessageResponseCreator;
    private readonly UserDeleteServiceValidator _validator;

    public UserDeleteServiceValidatorTests()
    {
        _mockMessageResponseCreator = new Mock<IMessageResponseCreator>();
        _validator = new UserDeleteServiceValidator(_mockMessageResponseCreator.Object);
    }

    [Fact]
    public async Task Validate_UserIsNull_ReturnsNotFound()
    {
        // Arrange
        _mockMessageResponseCreator.Setup(m => m.Create(StatusCodeType.NotFound, It.IsAny<string>()))
            .Returns(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.NotFound,
                Data = new MessageDto(Resources.UserNotFoundMessage)
            });

        // Act
        var result = await _validator.Validate(null);

        // Assert
        Assert.Equal(StatusCodeType.NotFound, result.StatusCode);
        Assert.Equal(Resources.UserNotFoundMessage, result.Data.Message);
    }

    [Fact]
    public async Task Validate_UserIsNotNull_ReturnsSuccess()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com"
        };

        _mockMessageResponseCreator.Setup(m => m.Create(StatusCodeType.Success, It.IsAny<string>()))
            .Returns(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.Success,
                Data = new MessageDto(Resources.SuccessfulDeleteUserMessage)
            });

        // Act
        var result = await _validator.Validate(user);

        // Assert
        Assert.Equal(StatusCodeType.Success, result.StatusCode);
        Assert.Equal(Resources.SuccessfulDeleteUserMessage, result.Data.Message);
    }
}