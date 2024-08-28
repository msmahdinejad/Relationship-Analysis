using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.CRUD.Role;
using RelationshipAnalysis.Services.Panel.AdminPanelServices;

namespace RelationshipAnalysis.Test.Services.Panel.AdminPanelServices;

public class UserRolesReceiverTests
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly RoleReceiver _sut;

    public UserRolesReceiverTests()
    {
        _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(_ => _context);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new RoleReceiver(_serviceProvider);
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var roles = new List<Role>
        {
            new() { Id = 1, Name = "Admin", Permissions = "" },
            new() { Id = 2, Name = "User", Permissions = "" }
        };

        var users = new List<User>
        {
            new() { Id = 1, Username = "User1", Email = "", FirstName = "", LastName = "", PasswordHash = "" },
            new() { Id = 2, Username = "User2", Email = "", FirstName = "", LastName = "", PasswordHash = "" }
        };

        var userRoles = new List<UserRole>
        {
            new() { UserId = 1, RoleId = 1 },
            new() { UserId = 1, RoleId = 2 },
            new() { UserId = 2, RoleId = 2 }
        };

        _context.Roles.AddRange(roles);
        _context.Users.AddRange(users);
        _context.UserRoles.AddRange(userRoles);
        _context.SaveChanges();
    }

    [Fact]
    public async Task ReceiveRoles_ReturnsRoles_WhenUserIdIsValid()
    {
        // Arrange
        var userId = 1;

        // Act
        var result = await _sut.ReceiveRoleNamesAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains("Admin", result);
        Assert.Contains("User", result);
    }

    [Fact]
    public async Task ReceiveRoles_ReturnsEmptyList_WhenUserIdHasNoRoles()
    {
        // Arrange
        var userId = 3; // Non-existent user ID

        // Act
        var result = await _sut.ReceiveRoleNamesAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ReceiveRoles_ReturnsEmptyList_WhenUserIdIsInvalid()
    {
        // Arrange
        var userId = 99; // Invalid user ID

        // Act
        var result = await _sut.ReceiveRoleNamesAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}