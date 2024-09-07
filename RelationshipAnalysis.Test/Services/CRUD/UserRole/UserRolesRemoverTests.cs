using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CRUD.UserRole;

namespace RelationshipAnalysis.Test.Services.CRUD.UserRole;

public class UserRolesRemoverTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly UserRolesRemover _sut;

    public UserRolesRemoverTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseLazyLoadingProxies()
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _sut = new UserRolesRemover(_serviceProvider);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(new Models.Auth.User
        {
            Id = 1,
            Username = "existinguser",
            Email = "existing@example.com",
            PasswordHash = "hashedpassword",
            FirstName = "Jane",
            LastName = "Doe"
        });

        context.Roles.AddRange(new List<Models.Auth.Role>
        {
            new() { Id = 1, Name = "Admin", Permissions = "[]" },
            new() { Id = 2, Name = "User", Permissions = "[]" }
        });

        context.UserRoles.AddRange(new List<Models.Auth.UserRole>
        {
            new() { UserId = 1, RoleId = 1 },
            new() { UserId = 1, RoleId = 2 }
        });

        context.SaveChanges();
    }

    [Fact]
    public async Task RemoveUserRoles_ShouldRemoveRolesFromUser()
    {
        // Arrange
        var user = new Models.Auth.User { Id = 1 };

        // Act
        await _sut.RemoveUserRoles(user);

        // Assert
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userRoles = await context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .ToListAsync();

        Assert.Empty(userRoles);
    }

    [Fact]
    public async Task RemoveUserRoles_ShouldNotAffectOtherUsers()
    {
        // Arrange
        var user1 = new Models.Auth.User { Id = 1 };
        var user2 = new Models.Auth.User { Id = 2 };

        // Act
        await _sut.RemoveUserRoles(user1);

        // Assert
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user2Roles = await context.UserRoles
            .Where(ur => ur.UserId == user2.Id)
            .ToListAsync();

        Assert.Empty(user2Roles);
    }
}