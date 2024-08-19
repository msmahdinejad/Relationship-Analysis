
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.AdminPanelServices;

namespace RelationshipAnalysis.Test.Services
{
    public class UserRolesReceiverTests
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleReceiver _sut;
        private readonly IServiceProvider _serviceProvider;
        public UserRolesReceiverTests()
        {
            _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options);
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
                new Role { Id = 1, Name = "Admin", Permissions = ""},
                new Role { Id = 2, Name = "User", Permissions = ""}
            };

            var users = new List<User>
            {
                new User { Id = 1, Username = "User1", Email = "", FirstName = "", LastName = "", PasswordHash = "" },
                new User { Id = 2, Username = "User2", Email = "", FirstName = "", LastName = "", PasswordHash = "" }
            };

            var userRoles = new List<UserRole>
            {
                new UserRole { UserId = 1, RoleId = 1 },
                new UserRole { UserId = 1, RoleId = 2 }, 
                new UserRole { UserId = 2, RoleId = 2 }  
            };

            _context.Roles.AddRange(roles);
            _context.Users.AddRange(users);
            _context.UserRoles.AddRange(userRoles);
            _context.SaveChanges();
        }

        [Fact]
        public void ReceiveRoles_ReturnsRoles_WhenUserIdIsValid()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = _sut.ReceiveRoles(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains("Admin", result);
            Assert.Contains("User", result);
        }

        [Fact]
        public void ReceiveRoles_ReturnsEmptyList_WhenUserIdHasNoRoles()
        {
            // Arrange
            var userId = 3; // Non-existent user ID

            // Act
            var result = _sut.ReceiveRoles(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ReceiveRoles_ReturnsEmptyList_WhenUserIdIsInvalid()
        {
            // Arrange
            var userId = 99; // Invalid user ID

            // Act
            var result = _sut.ReceiveRoles(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
