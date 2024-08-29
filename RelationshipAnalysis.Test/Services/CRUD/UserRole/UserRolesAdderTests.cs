using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CRUD.UserRole;

namespace RelationshipAnalysis.Test.Services.CRUD.UserRole;

public class UserRolesAdderTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly UserRolesAdder _sut;

        public UserRolesAdderTests()
        {
            var serviceCollection = new ServiceCollection();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseLazyLoadingProxies()
                .Options;

            serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _sut = new UserRolesAdder(_serviceProvider);

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
                new Models.Auth.Role { Id = 1, Name = "Admin", Permissions = "[]" },
                new Models.Auth.Role { Id = 2, Name = "User", Permissions = "[]" }
            });
            context.SaveChanges();
        }

        [Fact]
        public async Task AddUserRoles_ShouldAddRolesToUser()
        {
            // Arrange
            var user = new Models.Auth.User { Id = 1 };
            var roles = new List<Models.Auth.Role>
            {
                new Models.Auth.Role { Id = 1, Name = "Admin" },
                new Models.Auth.Role { Id = 2, Name = "User" }
            };

            // Act
            await _sut.AddUserRoles(roles, user);

            // Assert
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userRoles = await context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .ToListAsync();

            Assert.Equal(roles.Count, userRoles.Count);
            Assert.Contains(userRoles, ur => ur.RoleId == 1);
            Assert.Contains(userRoles, ur => ur.RoleId == 2);
        }
    }