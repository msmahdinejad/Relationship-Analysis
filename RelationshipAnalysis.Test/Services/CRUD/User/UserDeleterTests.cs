using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CRUD.User;

namespace RelationshipAnalysis.Test.Services.CRUD.User;

public class UserDeleterTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly UserDeleter _sut;

    public UserDeleterTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseLazyLoadingProxies()
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _sut = new UserDeleter(_serviceProvider);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.AddRange(new List<Models.Auth.User>
        {
            new()
            {
                Id = 1, Username = "user1", Email = "user1@example.com", PasswordHash = "hash1", FirstName = "User",
                LastName = "One"
            },
            new()
            {
                Id = 2, Username = "user2", Email = "user2@example.com", PasswordHash = "hash2", FirstName = "User",
                LastName = "Two"
            }
        });

        context.SaveChanges();
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldRemoveUserFromDatabase()
    {
        // Arrange
        Models.Auth.User userToDelete;
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            userToDelete = context.Users.First();
        }

        // Act
        await _sut.DeleteUserAsync(userToDelete);

        // Assert
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var deletedUser = await context.Users.FindAsync(userToDelete.Id);
            Assert.Null(deletedUser);
        }
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldNotAffectOtherUsers()
    {
        // Arrange
        Models.Auth.User userToDelete;
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            userToDelete = context.Users.First();
        }

        // Act
        await _sut.DeleteUserAsync(userToDelete);

        // Assert
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var remainingUsers = await context.Users.ToListAsync();
            Assert.Single(remainingUsers);
            Assert.NotEqual(userToDelete.Id, remainingUsers.First().Id);
        }
    }
}