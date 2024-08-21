using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.UserPanelServices;

namespace RelationshipAnalysis.Test.Services;

public class PermissionServiceTests
{
    private readonly PermissionService _sut;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<string> _userRoles = new List<string> { "Read", "Write" };
    private readonly List<string> _adminRoles = new List<string> { "Delete", "Write" };

    public PermissionServiceTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        SeedDatabase();

        _sut = new PermissionService(_serviceProvider);
    }

    private void SeedDatabase()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Roles.AddRange(new List<Role>
            {
                new Role
                {
                    Name = "User",
                    Permissions = JsonConvert.SerializeObject(_userRoles)
                },
                new Role
                {
                    Name = "Admin",
                    Permissions = JsonConvert.SerializeObject(_adminRoles)
                }
            });
            context.SaveChanges();
        }
    }

    [Fact]
    public async Task GetPermissionsAsync_ShouldReturnPermissions_WhenRolesExist()
    {
        // Arrange
        var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.Role, "Admin")
        }));

        // Act
        var result = await _sut.GetPermissionsAsync(userClaims);

        var expectedResult = _userRoles.Union(_adminRoles);
        var expectedPermissions = JsonConvert.SerializeObject(expectedResult);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(expectedPermissions, result.Data.Permissions);
    }
}