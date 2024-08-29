using Moq;
using Newtonsoft.Json;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.CRUD.Permissions.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.PermissionsService;

namespace RelationshipAnalysis.Test.Services.Panel.UserPanelServices.PermissionsService;

public class PermissionServiceTests
{
    private readonly Mock<IPermissionsReceiver> _permissionsReceiverMock;
    private readonly PermissionService _sut;

    public PermissionServiceTests()
    {
        _permissionsReceiverMock = new Mock<IPermissionsReceiver>();
        _sut = new PermissionService(_permissionsReceiverMock.Object);
    }

    [Fact]
    public async Task GetPermissionsAsync_ShouldReturnPermissionsInDto()
    {
        // Arrange
        var user = new User { Id = 1, Username = "testuser" };
        var permissionsList = new List<string> { "Read", "Write" };
        var serializedPermissions = JsonConvert.SerializeObject(permissionsList);

        _permissionsReceiverMock
            .Setup(p => p.ReceivePermissionsAsync(user))
            .ReturnsAsync(permissionsList);

        // Act
        var result = await _sut.GetPermissionsAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(serializedPermissions, result.Data.Permissions);
    }
}