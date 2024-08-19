using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.UserPanelServices;

namespace RelationshipAnalysis.Test.Services
{
    public class UserReceiverTests
    {
        private readonly ApplicationDbContext _context;
        private readonly UserReceiver _sut;
        private readonly IServiceProvider _serviceProvider;

        public UserReceiverTests()
        {
            _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options);
            
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped(_ => _context);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            
            _sut = new UserReceiver(_serviceProvider);
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _context.Users.AddRange(
                new User { Id = 1, Username = "User1" , Email = "", FirstName = "", LastName = "", PasswordHash = ""},
                new User { Id = 2, Username = "User2", Email = "", FirstName = "", LastName = "", PasswordHash = ""}
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task ReceiveUserAsync_ReturnsUser_WhenClaimsPrincipalHasValidId()
        {
            // Arrange
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }));

            // Act
            var result = await _sut.ReceiveUserAsync(claims);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("User1", result.Username);
        }

        [Fact]
        public async Task ReceiveUserAsync_ReturnsNull_WhenClaimsPrincipalHasInvalidId()
        {
            // Arrange
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "99") // Non-existent user ID
            }));

            // Act
            var result = await _sut.ReceiveUserAsync(claims);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ReceiveUserAsync_ReturnsUser_WhenIdIsValid()
        {
            // Arrange
            var userId = 2;

            // Act
            var result = await _sut.ReceiveUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("User2", result.Username);
        }

        [Fact]
        public async Task ReceiveUserAsync_ReturnsNull_WhenIdIsInvalid()
        {
            // Arrange
            var userId = 99; // Non-existent user ID

            // Act
            var result = await _sut.ReceiveUserAsync(userId);

            // Assert
            Assert.Null(result);
        }
    }
}
