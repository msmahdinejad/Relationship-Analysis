using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService.Abstraction;

namespace RelationshipAnalysis.Test.Services.Panel.UserPanelServices.UserUpdateInfoService;

public class UserUpdateInfoServiceTests
{
    private readonly Mock<IUserUpdateInfoServiceValidator> _validatorMock;
    private readonly Mock<IUserUpdater> _userUpdaterMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService.UserUpdateInfoService _sut;

    public UserUpdateInfoServiceTests()
    {
        _validatorMock = new Mock<IUserUpdateInfoServiceValidator>();
        _userUpdaterMock = new Mock<IUserUpdater>();
        _mapperMock = new Mock<IMapper>();

        _sut = new RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService.UserUpdateInfoService(_validatorMock.Object, _userUpdaterMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task UpdateUserAsync_InvalidUserUpdateInfo_ShouldReturnValidationError()
    {
        // Arrange
        var user = new User { Id = 1, Username = "username", Email = "email@example.com" };
        var userUpdateInfoDto = new UserUpdateInfoDto
        {
            Username = "newusername",
            Email = "newemail@example.com",
            FirstName = "New",
            LastName = "User"
        };

        var validationErrorResponse = new ActionResponse<MessageDto>
        {
            StatusCode = StatusCodeType.BadRequest,
            Data = new MessageDto("Validation Error")
        };

        _validatorMock
            .Setup(v => v.Validate(user, userUpdateInfoDto))
            .ReturnsAsync(validationErrorResponse);

        // Act
        var result = await _sut.UpdateUserAsync(user, userUpdateInfoDto, new DefaultHttpContext().Response);

        // Assert
        result.StatusCode.Should().Be(StatusCodeType.BadRequest);
        result.Data.Message.Should().Be("Validation Error");
        _userUpdaterMock.Verify(u => u.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserAsync_ValidUserUpdateInfo_ShouldUpdateUserAndReturnSuccess()
    {
        // Arrange
        var user = new User { Id = 1, Username = "username", Email = "email@example.com" };
        var userUpdateInfoDto = new UserUpdateInfoDto
        {
            Username = "newusername",
            Email = "newemail@example.com",
            FirstName = "New",
            LastName = "User"
        };

        var validationSuccessResponse = new ActionResponse<MessageDto>
        {
            StatusCode = StatusCodeType.Success,
            Data = new MessageDto("Update Successful")
        };

        _validatorMock
            .Setup(v => v.Validate(user, userUpdateInfoDto))
            .ReturnsAsync(validationSuccessResponse);

        _mapperMock
            .Setup(m => m.Map(userUpdateInfoDto, user))
            .Verifiable();

        _userUpdaterMock
            .Setup(u => u.UpdateUserAsync(user))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await _sut.UpdateUserAsync(user, userUpdateInfoDto, new DefaultHttpContext().Response);

        // Assert
        result.StatusCode.Should().Be(StatusCodeType.Success);
        result.Data.Message.Should().Be("Update Successful");
        _mapperMock.Verify();
        _userUpdaterMock.Verify();
    }
}