
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Test.Services
{
    public class UserUpdateInfoServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICookieSetter _cookieSetter;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly UserUpdateInfoService _sut;

        public UserUpdateInfoServiceTests()
        {
            _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options);

            _mapper = Substitute.For<IMapper>();
            _cookieSetter = Substitute.For<ICookieSetter>();
            _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
            _sut = new UserUpdateInfoService(_context, _mapper);
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _context.Users.AddRange(new User
            {
                Id = 1,
                Username = "ExistingUser",
                Email = "user@example.com",
                FirstName = "",
                LastName = "",
                PasswordHash = "HashedPassword"
            },
            new User
            {
                Id = 2,
                Username = "NewUserName",
                Email = "newemail@example.com",
                FirstName = "",
                LastName = "",
                PasswordHash = "HashedPassword"
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsBadRequest_WhenEmailAlreadyExists()
        {
            // Arrange
            var user = await _context.Users.FindAsync(1);
            var userUpdateInfoDto = new UserUpdateInfoDto
            {
                Username = "ExistingUser", 
                Email = "newemail@example.com"
            };
            var response = Substitute.For<HttpResponse>();

            // Act
            var result = await _sut.UpdateUserAsync(user, userUpdateInfoDto, response);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
            Assert.Equal(Resources.EmailExistsMessage, result.Data.Message);
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsBadRequest_WhenUsernameAlreadyExists()
        {
            // Arrange
            var user = await _context.Users.FindAsync(1);
            var userUpdateInfoDto = new UserUpdateInfoDto
            {
                Username = "NewUserName",
                Email = "user@example.com" 
            };
            var response = Substitute.For<HttpResponse>();

            // Act
            var result = await _sut.UpdateUserAsync(user, userUpdateInfoDto, response);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
            Assert.Equal(Resources.UsernameExistsMessage, result.Data.Message);
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsNotFound_WhenUserIsNull()
        {
            // Arrange
            User user = null;
            var userUpdateInfoDto = new UserUpdateInfoDto();
            var response = Substitute.For<HttpResponse>();

            // Act
            var result = await _sut.UpdateUserAsync(user, userUpdateInfoDto, response);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodeType.NotFound, result.StatusCode);
            Assert.Equal(Resources.UserNotFoundMessage, result.Data.Message);
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsSuccess_WhenUserIsUpdated()
        {
            // Arrange
            var user = await _context.Users.FindAsync(1);
            var userUpdateInfoDto = new UserUpdateInfoDto
            {
                Username = "UpdatedUserName",
                Email = "updated@example.com"
            };
            var resultUser = new User()
            {
                Id = 1,
                Username = "UpdatedUserName",
                Email = "updated@example.com",
                FirstName = "",
                LastName = "",
                PasswordHash = "HashedPassword"
            };
            _mapper.Map<User>(userUpdateInfoDto).Returns(resultUser);
            
            var response = Substitute.For<HttpResponse>();
            
            // Act
            var result = await _sut.UpdateUserAsync(user, userUpdateInfoDto, response);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodeType.Success, result.StatusCode);
            Assert.Equal(Resources.SuccessfulUpdateUserMessage, result.Data.Message);
            Assert.Equal("ExistingUser", user.Username);
            Assert.Equal("user@example.com", user.Email);
        }
        
    }
}
