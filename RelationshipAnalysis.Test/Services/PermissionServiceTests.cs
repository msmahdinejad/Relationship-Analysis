using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.AccessServices;

namespace RelationshipAnalysis.Test.Services;

public class PermissionServiceTests
{
    private readonly PermissionService _sut;
    private readonly ApplicationDbContext _context;
    private readonly List<string> _userRoles = ["Read", "Write"];
    private readonly List<string> _adminRoles = ["Delete", "Write"];

    public PermissionServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);

        SeedDatabase();

        _sut = new PermissionService(_context);
    }

    private void SeedDatabase()
    {
        _context.Roles.AddRange(new List<Role>
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
        _context.SaveChanges();
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

        // Assert
        var expectedPermissions = JsonConvert.SerializeObject(expectedResult);
        Assert.Equal(expectedPermissions, result.Data.Permissions);
    }
}