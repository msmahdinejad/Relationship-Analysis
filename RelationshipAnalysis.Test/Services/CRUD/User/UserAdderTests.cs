using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CRUD.User;

namespace RelationshipAnalysis.Test.Services.CRUD.User;

public class UserAdderTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly UserAdder _sut;

    public UserAdderTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseLazyLoadingProxies()
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _sut = new UserAdder(_serviceProvider);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Users.AddRange(new List<Models.Auth.User>
        {
            new Models.Auth.User
            {
                Id = 1,
                Username = "existinguser1",
                Email = "existing1@example.com",
                PasswordHash = "hashedpassword1",
                FirstName = "Jane",
                LastName = "Doe"
            },
            new Models.Auth.User
            {
                Id = 2,
                Username = "existinguser2",
                Email = "existing2@example.com",
                PasswordHash = "hashedpassword2",
                FirstName = "John",
                LastName = "Smith"
            }
        });
        context.SaveChanges();
    }

    [Fact]
    public async Task AddUserAsync_ShouldAddUserToDatabase()
    {
        // Arrange
        var user = new Models.Auth.User
        {
            Id = 3,
            Username = "newuser",
            Email = "newuser@example.com",
            PasswordHash = "newhashedpassword",
            FirstName = "New",
            LastName = "User"
        };

        // Act
        await _sut.AddUserAsync(user);

        // Assert
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var addedUser = await context.Users.FindAsync(user.Id);
        Assert.NotNull(addedUser);
        Assert.Equal(user.Username, addedUser.Username);
    }

    [Fact]
    public async Task AddUserAsync_ShouldSaveChanges()
    {
        // Arrange
        var user = new Models.Auth.User
        {
            Id = 4,
            Username = "anotheruser",
            Email = "anotheruser@example.com",
            PasswordHash = "anotherhashedpassword",
            FirstName = "Another",
            LastName = "User"
        };

        // Act
        await _sut.AddUserAsync(user);

        // Assert
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var addedUser = await context.Users.FindAsync(user.Id);
        Assert.NotNull(addedUser);
    }
}