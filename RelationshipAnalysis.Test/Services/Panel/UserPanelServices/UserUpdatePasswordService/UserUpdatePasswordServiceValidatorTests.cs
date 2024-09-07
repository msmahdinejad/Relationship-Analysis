using FluentAssertions;
using Moq;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.AuthServices.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdatePasswordService;

namespace RelationshipAnalysis.Test.Services.Panel.UserPanelServices.UserUpdatePasswordService;

public class UserUpdatePasswordServiceValidatorTests
{
    private readonly Mock<IMessageResponseCreator> _messageResponseCreatorMock;
    private readonly Mock<IPasswordVerifier> _passwordVerifierMock;
    private readonly UserUpdatePasswordServiceValidator _sut;

    public UserUpdatePasswordServiceValidatorTests()
    {
        _passwordVerifierMock = new Mock<IPasswordVerifier>();
        _messageResponseCreatorMock = new Mock<IMessageResponseCreator>();

        _sut = new UserUpdatePasswordServiceValidator(_passwordVerifierMock.Object, _messageResponseCreatorMock.Object);
    }

    [Fact]
    public async Task Validate_UserIsNull_ShouldReturnNotFoundMessage()
    {
        // Arrange
        var passwordInfoDto = new UserPasswordInfoDto
        {
            OldPassword = "oldpassword"
        };

        var expectedResponse = new ActionResponse<MessageDto>
        {
            StatusCode = StatusCodeType.NotFound,
            Data = new MessageDto(Resources.UserNotFoundMessage)
        };

        _messageResponseCreatorMock
            .Setup(m => m.Create(StatusCodeType.NotFound, Resources.UserNotFoundMessage))
            .Returns(expectedResponse);

        // Act
        var result = await _sut.Validate(null, passwordInfoDto);

        // Assert
        result.StatusCode.Should().Be(StatusCodeType.NotFound);
        result.Data.Message.Should().Be(Resources.UserNotFoundMessage);
    }

    [Fact]
    public async Task Validate_WrongOldPassword_ShouldReturnWrongOldPasswordMessage()
    {
        // Arrange
        var user = new User
        {
            PasswordHash = "correcthash"
        };

        var passwordInfoDto = new UserPasswordInfoDto
        {
            OldPassword = "wrongpassword"
        };

        _passwordVerifierMock
            .Setup(p => p.VerifyPasswordHash(passwordInfoDto.OldPassword, user.PasswordHash))
            .Returns(false);

        var expectedResponse = new ActionResponse<MessageDto>
        {
            StatusCode = StatusCodeType.BadRequest,
            Data = new MessageDto(Resources.WrongOldPasswordMessage)
        };

        _messageResponseCreatorMock
            .Setup(m => m.Create(StatusCodeType.BadRequest, Resources.WrongOldPasswordMessage))
            .Returns(expectedResponse);

        // Act
        var result = await _sut.Validate(user, passwordInfoDto);

        // Assert
        result.StatusCode.Should().Be(StatusCodeType.BadRequest);
        result.Data.Message.Should().Be(Resources.WrongOldPasswordMessage);
    }

    [Fact]
    public async Task Validate_CorrectOldPassword_ShouldReturnSuccessMessage()
    {
        // Arrange
        var user = new User
        {
            PasswordHash = "correcthash"
        };

        var passwordInfoDto = new UserPasswordInfoDto
        {
            OldPassword = "correctpassword"
        };

        _passwordVerifierMock
            .Setup(p => p.VerifyPasswordHash(passwordInfoDto.OldPassword, user.PasswordHash))
            .Returns(true);

        var expectedResponse = new ActionResponse<MessageDto>
        {
            StatusCode = StatusCodeType.Success,
            Data = new MessageDto(Resources.SuccessfulUpdateUserMessage)
        };

        _messageResponseCreatorMock
            .Setup(m => m.Create(StatusCodeType.Success, Resources.SuccessfulUpdateUserMessage))
            .Returns(expectedResponse);

        // Act
        var result = await _sut.Validate(user, passwordInfoDto);

        // Assert
        result.StatusCode.Should().Be(StatusCodeType.Success);
        result.Data.Message.Should().Be(Resources.SuccessfulUpdateUserMessage);
    }
}