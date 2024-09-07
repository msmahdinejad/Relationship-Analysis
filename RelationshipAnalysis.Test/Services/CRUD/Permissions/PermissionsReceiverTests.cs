using Moq;
using Newtonsoft.Json;
using RelationshipAnalysis.Services.CRUD.Permissions;
using RelationshipAnalysis.Services.CRUD.Role.Abstraction;

namespace RelationshipAnalysis.Test.Services.CRUD.Permissions;

public class PermissionsReceiverTests
{
    private readonly List<string> _expectedPermissions = ["Read", "Write", "Delete"];
    private readonly Mock<IRoleReceiver> _roleReceiverMock;
    private readonly PermissionsReceiver _sut;

    public PermissionsReceiverTests()
    {
        _roleReceiverMock = new Mock<IRoleReceiver>();
        _sut = new PermissionsReceiver(_roleReceiverMock.Object);

        SetupRoleReceiverMock();
    }

    private void SetupRoleReceiverMock()
    {
        _roleReceiverMock.Setup(r => r.ReceiveRoleNamesAsync(It.IsAny<int>()))
            .ReturnsAsync(["Admin", "User"]);

        _roleReceiverMock.Setup(r => r.ReceiveRolesListAsync(It.IsAny<List<string>>()))
            .ReturnsAsync([
                new Models.Auth.Role
                {
                    Id = 1,
                    Name = "Admin",
                    Permissions = JsonConvert.SerializeObject(_expectedPermissions)
                },

                new Models.Auth.Role
                {
                    Id = 2,
                    Name = "User",
                    Permissions = JsonConvert.SerializeObject(new List<string> { "Read" })
                }
            ]);
    }

    [Fact]
    public async Task ReceivePermissionsAsync_ShouldReturnUnionOfPermissions()
    {
        // Arrange
        var user = new Models.Auth.User { Id = 1 };

        // Act
        var permissions = await _sut.ReceivePermissionsAsync(user);

        // Assert
        Assert.Equal(_expectedPermissions.Count, permissions.Count);
        Assert.True(_expectedPermissions.All(permission => permissions.Contains(permission)));
    }

    [Fact]
    public async Task ReceivePermissionsAsync_ShouldHandleEmptyPermissions()
    {
        // Arrange
        _roleReceiverMock.Setup(r => r.ReceiveRolesListAsync(It.IsAny<List<string>>()))
            .ReturnsAsync([]);

        var user = new Models.Auth.User { Id = 1 };

        // Act
        var permissions = await _sut.ReceivePermissionsAsync(user);

        // Assert
        Assert.Empty(permissions);
    }
}