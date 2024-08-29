using Moq;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.CRUD.Role.Abstraction;
using RelationshipAnalysis.Services.CRUD.UserRole.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.UserUpdateRolesService.Abstraction;

namespace RelationshipAnalysis.Test.Services.Panel.AdminPanelServices.UserUpdateRolesService;

public class UserUpdateRolesServiceTests
{
    private readonly Mock<IUserUpdateRolesServiceValidator> _mockValidator;
    private readonly Mock<IRoleReceiver> _mockRoleReceiver;
    private readonly Mock<IUserRolesAdder> _mockUserRolesAdder;
    private readonly Mock<IUserRolesRemover> _mockUserRolesRemover;
    private readonly RelationshipAnalysis.Services.Panel.AdminPanelServices.UserUpdateRolesService.UserUpdateRolesService _service;

    public UserUpdateRolesServiceTests()
    {
        _mockValidator = new Mock<IUserUpdateRolesServiceValidator>();
        _mockRoleReceiver = new Mock<IRoleReceiver>();
        _mockUserRolesAdder = new Mock<IUserRolesAdder>();
        _mockUserRolesRemover = new Mock<IUserRolesRemover>();

        _service = new RelationshipAnalysis.Services.Panel.AdminPanelServices.UserUpdateRolesService.UserUpdateRolesService(
            _mockValidator.Object,
            _mockRoleReceiver.Object,
            _mockUserRolesAdder.Object,
            _mockUserRolesRemover.Object);
    }

    [Fact]
    public async Task UpdateUserRolesAsync_ValidationFails_ReturnsValidationFailure()
    {
        // Arrange
        var user = new User();
        var newRoles = new List<string> { "Admin", "User" };

        _mockValidator.Setup(v => v.Validate(user, newRoles))
            .ReturnsAsync(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.BadRequest,
                Data = new MessageDto("Validation failed")
            });

        // Act
        var result = await _service.UpdateUserRolesAsync(user, newRoles);

        // Assert
        Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        _mockUserRolesRemover.Verify(r => r.RemoveUserRoles(It.IsAny<User>()), Times.Never);
        _mockUserRolesAdder.Verify(a => a.AddUserRoles(It.IsAny<List<Role>>(), It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserRolesAsync_ValidationSucceeds_UpdatesRolesSuccessfully()
    {
        // Arrange
        var user = new User();
        var newRoles = new List<string> { "Admin", "User" };
        var roleList = new List<Role>
        {
            new Role { Name = "Admin" },
            new Role { Name = "User" }
        };

        _mockValidator.Setup(v => v.Validate(user, newRoles))
            .ReturnsAsync(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.Success
            });

        _mockRoleReceiver.Setup(r => r.ReceiveRolesListAsync(newRoles))
            .ReturnsAsync(roleList);

        _mockUserRolesRemover.Setup(r => r.RemoveUserRoles(user)).Returns(Task.CompletedTask);
        _mockUserRolesAdder.Setup(a => a.AddUserRoles(roleList, user)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateUserRolesAsync(user, newRoles);

        // Assert
        Assert.Equal(StatusCodeType.Success, result.StatusCode);
        _mockUserRolesRemover.Verify(r => r.RemoveUserRoles(user), Times.Once);
        _mockUserRolesAdder.Verify(a => a.AddUserRoles(roleList, user), Times.Once);
    }
}