using Moq;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService.Abstraction;

namespace RelationshipAnalysis.Test.Services.Panel.UserPanelServices.UserInfoService;

public class UserInfoServiceTests
{
    private readonly RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService.UserInfoService _sut;
    private readonly Mock<IUserOutputInfoDtoCreator> _userOutputInfoDtoCreatorMock;
    private readonly Mock<IUserInfoServiceValidator> _validatorMock;

    public UserInfoServiceTests()
    {
        _userOutputInfoDtoCreatorMock = new Mock<IUserOutputInfoDtoCreator>();
        _validatorMock = new Mock<IUserInfoServiceValidator>();
        _sut = new RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService.UserInfoService(
            _userOutputInfoDtoCreatorMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task GetUser_ValidationFails_ShouldReturnValidationResult()
    {
        // Arrange
        var user = new User { Id = 1, Username = "testuser" };
        var validationResponse = new ActionResponse<UserOutputInfoDto>
        {
            StatusCode = StatusCodeType.NotFound
        };

        _validatorMock
            .Setup(v => v.Validate(user))
            .ReturnsAsync(validationResponse);

        // Act
        var result = await _sut.GetUser(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodeType.NotFound, result.StatusCode);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetUser_ValidationSucceeds_ShouldReturnUserOutputInfoDto()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            FirstName = "Test",
            LastName = "User"
        };

        var dto = new UserOutputInfoDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = new List<string> { "Role1", "Role2" }
        };

        var validationResponse = new ActionResponse<UserOutputInfoDto>
        {
            StatusCode = StatusCodeType.Success
        };

        _validatorMock
            .Setup(v => v.Validate(user))
            .ReturnsAsync(validationResponse);

        _userOutputInfoDtoCreatorMock
            .Setup(c => c.Create(user))
            .ReturnsAsync(dto);

        // Act
        var result = await _sut.GetUser(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodeType.Success, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal(user.Id, result.Data.Id);
        Assert.Equal(user.Username, result.Data.Username);
        Assert.Equal(user.Email, result.Data.Email);
        Assert.Equal(user.FirstName, result.Data.FirstName);
        Assert.Equal(user.LastName, result.Data.LastName);
        Assert.Contains("Role1", result.Data.Roles);
        Assert.Contains("Role2", result.Data.Roles);
        Assert.Equal(2, result.Data.Roles.Count);
    }
}