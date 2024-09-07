using FluentAssertions;
using Moq;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.AuthServices.Abstraction;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdatePasswordService.Abstraction;

namespace RelationshipAnalysis.Test.Services.Panel.UserPanelServices.UserUpdatePasswordService;

public class UserUpdatePasswordServiceTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock;

    private readonly RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdatePasswordService.
        UserUpdatePasswordService _sut;

    private readonly Mock<IUserUpdater> _userUpdaterMock;
    private readonly Mock<IUserUpdatePasswordServiceValidator> _validatorMock;

    public UserUpdatePasswordServiceTests()
    {
        _validatorMock = new Mock<IUserUpdatePasswordServiceValidator>();
        _userUpdaterMock = new Mock<IUserUpdater>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _sut =
            new RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdatePasswordService.
                UserUpdatePasswordService(_validatorMock.Object, _userUpdaterMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task UpdatePasswordAsync_ValidationFails_ShouldReturnValidationResult()
    {
        // Arrange
        var user = new User { PasswordHash = "oldhash" };
        var passwordInfoDto = new UserPasswordInfoDto
        {
            OldPassword = "oldpassword",
            NewPassword = "newpassword"
        };

        var validationResponse = new ActionResponse<MessageDto>
        {
            StatusCode = StatusCodeType.BadRequest,
            Data = new MessageDto("Validation error")
        };

        _validatorMock
            .Setup(v => v.Validate(user, passwordInfoDto))
            .ReturnsAsync(validationResponse);

        // Act
        var result = await _sut.UpdatePasswordAsync(user, passwordInfoDto);

        // Assert
        result.StatusCode.Should().Be(StatusCodeType.BadRequest);
        result.Data.Message.Should().Be("Validation error");
    }

    [Fact]
    public async Task UpdatePasswordAsync_ValidationSucceeds_ShouldUpdatePasswordAndReturnSuccess()
    {
        // Arrange
        var user = new User { PasswordHash = "oldhash" };
        var passwordInfoDto = new UserPasswordInfoDto
        {
            OldPassword = "oldpassword",
            NewPassword = "newpassword"
        };

        var validationResponse = new ActionResponse<MessageDto>
        {
            StatusCode = StatusCodeType.Success,
            Data = new MessageDto("Password updated successfully")
        };

        _validatorMock
            .Setup(v => v.Validate(user, passwordInfoDto))
            .ReturnsAsync(validationResponse);

        _passwordHasherMock
            .Setup(p => p.HashPassword(passwordInfoDto.NewPassword))
            .Returns("newhash");

        // Act
        var result = await _sut.UpdatePasswordAsync(user, passwordInfoDto);

        // Assert
        _passwordHasherMock.Verify(p => p.HashPassword(passwordInfoDto.NewPassword), Times.Once);
        _userUpdaterMock.Verify(u => u.UpdateUserAsync(user), Times.Once);
        result.StatusCode.Should().Be(StatusCodeType.Success);
        result.Data.Message.Should().Be("Password updated successfully");
        user.PasswordHash.Should().Be("newhash");
    }

    [Fact]
    public async Task UpdatePasswordAsync_ValidatorFails_ShouldNotUpdatePassword()
    {
        // Arrange
        var user = new User { PasswordHash = "oldhash" };
        var passwordInfoDto = new UserPasswordInfoDto
        {
            OldPassword = "oldpassword",
            NewPassword = "newpassword"
        };

        var validationResponse = new ActionResponse<MessageDto>
        {
            StatusCode = StatusCodeType.BadRequest,
            Data = new MessageDto("Validation error")
        };

        _validatorMock
            .Setup(v => v.Validate(user, passwordInfoDto))
            .ReturnsAsync(validationResponse);

        // Act
        var result = await _sut.UpdatePasswordAsync(user, passwordInfoDto);

        // Assert
        _userUpdaterMock.Verify(u => u.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }
}