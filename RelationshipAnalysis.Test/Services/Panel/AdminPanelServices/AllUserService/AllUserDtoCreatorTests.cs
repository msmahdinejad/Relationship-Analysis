using AutoMapper;
using FluentAssertions;
using Moq;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.CRUD.Role.Abstraction;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService;

namespace RelationshipAnalysis.Test.Services.Panel.AdminPanelServices.AllUserService;

public class AllUserDtoCreatorTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRoleReceiver> _rolesReceiverMock;
    private readonly AllUserDtoCreator _sut;
    private readonly Mock<IUserReceiver> _userReceiverMock;

    public AllUserDtoCreatorTests()
    {
        _mapperMock = new Mock<IMapper>();
        _rolesReceiverMock = new Mock<IRoleReceiver>();
        _userReceiverMock = new Mock<IUserReceiver>();

        _sut = new AllUserDtoCreator(_mapperMock.Object, _rolesReceiverMock.Object, _userReceiverMock.Object);
    }

    [Fact]
    public async Task Create_ShouldMapUsersAndRetrieveRolesAndUserCount()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = 1, Username = "user1", Email = "user1@example.com" },
            new() { Id = 2, Username = "user2", Email = "user2@example.com" }
        };

        _mapperMock
            .Setup(m => m.Map(It.IsAny<User>(), It.IsAny<UserOutputInfoDto>()))
            .Returns((User source, UserOutputInfoDto dest) =>
            {
                dest.Id = source.Id;
                dest.Username = source.Username;
                dest.Email = source.Email;
                return dest;
            });

        _rolesReceiverMock
            .Setup(r => r.ReceiveRoleNamesAsync(It.IsAny<int>()))
            .ReturnsAsync((int userId) => userId == 1 ? ["Admin"] : ["User"]);

        _userReceiverMock
            .Setup(u => u.ReceiveAllUserCountAsync())
            .ReturnsAsync(2);

        // Act
        var result = await _sut.Create(users);

        // Assert
        result.Should().NotBeNull();
        result.Users.Should().HaveCount(users.Count);

        result.Users[0].Id.Should().Be(1);
        result.Users[0].Username.Should().Be("user1");
        result.Users[0].Email.Should().Be("user1@example.com");
        result.Users[0].Roles.Should().Contain("Admin");

        result.Users[1].Id.Should().Be(2);
        result.Users[1].Username.Should().Be("user2");
        result.Users[1].Email.Should().Be("user2@example.com");
        result.Users[1].Roles.Should().Contain("User");

        result.AllUserCount.Should().Be(2);
    }
}