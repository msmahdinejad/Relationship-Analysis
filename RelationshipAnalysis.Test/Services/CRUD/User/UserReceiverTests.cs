using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CRUD.User;

namespace RelationshipAnalysis.Test.Services.CRUD.User;

public class UserReceiverTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly UserReceiver _sut;

    public UserReceiverTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _sut = new UserReceiver(_serviceProvider);

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
    public async Task ReceiveAllUserCountAsync_ShouldReturnCorrectUserCount()
    {
        // Arrange

        // Act
        var userCount = await _sut.ReceiveAllUserCountAsync();

        // Assert
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var expectedCount = await context.Users.CountAsync();
        Assert.Equal(expectedCount, userCount);
    }

    [Fact]
    public async Task ReceiveUserAsync_ShouldReturnUserForClaimsPrincipal()
    {
        // Arrange
        var testUser = new Models.Auth.User
        {
            Id = 3,
            Username = "user3",
            Email = "user3@example.com",
            PasswordHash = "hashedpassword3",
            FirstName = "Alice",
            LastName = "Johnson"
        };

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Users.Add(testUser);
        await context.SaveChangesAsync();

        var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, testUser.Id.ToString())
        }));

        // Act
        var user = await _sut.ReceiveUserAsync(claims);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(testUser.Id, user.Id);
        Assert.Equal(testUser.Username, user.Username);
    }

    [Fact]
    public async Task ReceiveUserAsync_ShouldReturnUserById()
    {
        // Arrange
        var testUserId = 2;

        // Act
        var user = await _sut.ReceiveUserAsync(testUserId);

        // Assert
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var expectedUser = await context.Users.FindAsync(testUserId);

        Assert.NotNull(user);
        Assert.Equal(expectedUser.Id, user.Id);
        Assert.Equal(expectedUser.Username, user.Username);
    }

    [Fact]
    public async Task ReceiveUserAsync_ShouldReturnUserByUsername()
    {
        // Arrange
        var testUsername = "user1";

        // Act
        var user = await _sut.ReceiveUserAsync(testUsername);

        // Assert
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var expectedUser = await context.Users
            .SingleOrDefaultAsync(u => u.Username == testUsername);

        Assert.NotNull(user);
        Assert.Equal(expectedUser.Username, user.Username);
        Assert.Equal(expectedUser.Id, user.Id);
    }

    [Fact]
    public void ReceiveAllUserAsync_ShouldReturnPagedUsers()
    {
        // Arrange
        const int page = 0;
        const int size = 2;

        // Act
        var users = _sut.ReceiveAllUserAsync(page, size);

        // Assert
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var expectedUsers = context.Users
            .Skip(page * size)
            .Take(size)
            .ToList();

        Assert.NotNull(users);
        Assert.Equal(expectedUsers.Count, users.Count);
        Assert.Contains(users, u => u.Username == "user1");
        Assert.Contains(users, u => u.Username == "user2");
    }
}