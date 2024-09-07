using AutoMapper;
using Moq;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.CRUD.Role.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService;

namespace RelationshipAnalysis.Test.Services.Panel.UserPanelServices.UserInfoService;

public class UserOutputInfoDtoCreatorTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRoleReceiver> _roleReceiverMock;
    private readonly UserOutputInfoDtoCreator _sut;

    public UserOutputInfoDtoCreatorTests()
    {
        _mapperMock = new Mock<IMapper>();
        _roleReceiverMock = new Mock<IRoleReceiver>();
        _sut = new UserOutputInfoDtoCreator(_mapperMock.Object, _roleReceiverMock.Object);
    }

    [Fact]
    public async Task Create_ShouldMapUserToDtoAndRetrieveRoles()
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
        var roleNames = new List<string> { "Admin", "User" };

        _mapperMock
            .Setup(m => m.Map(user, It.IsAny<UserOutputInfoDto>()))
            .Callback<User, UserOutputInfoDto>((src, dest) =>
            {
                dest.Id = src.Id;
                dest.Username = src.Username;
                dest.Email = src.Email;
                dest.FirstName = src.FirstName;
                dest.LastName = src.LastName;
            });

        _roleReceiverMock
            .Setup(r => r.ReceiveRoleNamesAsync(user.Id))
            .ReturnsAsync(roleNames);

        // Act
        var result = await _sut.Create(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.FirstName, result.FirstName);
        Assert.Equal(user.LastName, result.LastName);
        Assert.NotNull(result.Roles);
        Assert.Contains("Admin", result.Roles);
        Assert.Contains("User", result.Roles);
        Assert.Equal(2, result.Roles.Count);
    }
}