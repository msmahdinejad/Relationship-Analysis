using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.CRUD.Role;

namespace RelationshipAnalysis.Test.Services.CRUD.Role;

public class RoleReceiverTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RoleReceiver _sut;

    public RoleReceiverTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseLazyLoadingProxies()
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _sut = new RoleReceiver(_serviceProvider);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Roles.AddRange(new List<Models.Auth.Role>
        {
            new Models.Auth.Role { Id = 1, Name = "Admin", Permissions = "" },
            new Models.Auth.Role { Id = 2, Name = "User", Permissions = "" }
        });

        context.Users.AddRange(new List<Models.Auth.User>
        {
            new Models.Auth.User
            {
                Id = 1, Username = "user1", Email = "user1@example.com", PasswordHash = "hash1", FirstName = "User",
                LastName = "One"
            },
            new Models.Auth.User
            {
                Id = 2, Username = "user2", Email = "user2@example.com", PasswordHash = "hash2", FirstName = "User",
                LastName = "Two"
            }
        });

        context.UserRoles.AddRange(new List<Models.Auth.UserRole>
        {
            new Models.Auth.UserRole { UserId = 1, RoleId = 1 },
            new Models.Auth.UserRole { UserId = 1, RoleId = 2 },
            new Models.Auth.UserRole { UserId = 2, RoleId = 2 }
        });

        context.SaveChanges();
    }

    [Fact]
    public async Task ReceiveRoleNamesAsync_ShouldReturnRoleNamesForUser()
    {
        // Arrange
        const int userId = 1;

        // Act
        var roleNames = await _sut.ReceiveRoleNamesAsync(userId);

        // Assert
        Assert.Contains("Admin", roleNames);
        Assert.Contains("User", roleNames);
        Assert.Equal(2, roleNames.Count);
    }

    [Fact]
    public async Task ReceiveAllRolesAsync_ShouldReturnAllRoleNames()
    {
        // Act
        var roleNames = await _sut.ReceiveAllRolesAsync();

        // Assert
        Assert.Contains("Admin", roleNames);
        Assert.Contains("User", roleNames);
        Assert.Equal(2, roleNames.Count);
    }

    [Fact]
    public async Task ReceiveRolesListAsync_ShouldReturnRolesForRoleNames()
    {
        // Arrange
        var roleNames = new List<string> { "Admin", "User" };

        // Act
        var roles = await _sut.ReceiveRolesListAsync(roleNames);

        // Assert
        Assert.Equal(2, roles.Count);
        Assert.Contains(roles, r => r.Name == "Admin");
        Assert.Contains(roles, r => r.Name == "User");
    }

    [Fact]
    public async Task ReceiveRolesListAsync_ShouldReturnEmptyForUnknownRoleNames()
    {
        // Arrange
        var roleNames = new List<string> { "NonExistentRole" };

        // Act
        var roles = await _sut.ReceiveRolesListAsync(roleNames);

        // Assert
        Assert.Empty(roles);
    }
}