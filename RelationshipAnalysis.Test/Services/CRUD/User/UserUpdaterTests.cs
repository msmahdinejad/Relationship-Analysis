using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CRUD.User;

namespace RelationshipAnalysis.Test.Services.CRUD.User;

public class UserUpdaterTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly UserUpdater _sut;

        public UserUpdaterTests()
        {
            var serviceCollection = new ServiceCollection();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _sut = new UserUpdater(_serviceProvider);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var testUsers = new List<Models.Auth.User>
            {
                new()
                {
                    Id = 1,
                    Username = "user1",
                    Email = "user1@example.com",
                    PasswordHash = "hashedpassword1",
                    FirstName = "Jane",
                    LastName = "Doe"
                },
                new()
                {
                    Id = 2,
                    Username = "user2",
                    Email = "user2@example.com",
                    PasswordHash = "hashedpassword2",
                    FirstName = "John",
                    LastName = "Smith"
                }
            };

            context.Users.AddRange(testUsers);
            context.SaveChanges();
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateExistingUser()
        {
            // Arrange
            var userToUpdate = new Models.Auth.User
            {
                Id = 1,
                Username = "user1_updated",
                Email = "user1_updated@example.com",
                PasswordHash = "newhashedpassword1",
                FirstName = "Jane",
                LastName = "Doe"
            };

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Users.Update(userToUpdate);
            await context.SaveChangesAsync();

            // Act
            await _sut.UpdateUserAsync(userToUpdate);

            // Assert
            var updatedUser = await context.Users.FindAsync(userToUpdate.Id);

            Assert.NotNull(updatedUser);
            Assert.Equal(userToUpdate.Username, updatedUser.Username);
            Assert.Equal(userToUpdate.Email, updatedUser.Email);
            Assert.Equal(userToUpdate.PasswordHash, updatedUser.PasswordHash);
        }
    }