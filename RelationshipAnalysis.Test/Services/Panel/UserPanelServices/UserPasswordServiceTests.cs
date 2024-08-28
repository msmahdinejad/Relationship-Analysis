// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using NSubstitute;
// using RelationshipAnalysis.Context;
// using RelationshipAnalysis.Dto.Panel.User;
// using RelationshipAnalysis.Enums;
// using RelationshipAnalysis.Models.Auth;
// using RelationshipAnalysis.Services.AuthServices.Abstraction;
// using RelationshipAnalysis.Services.Panel.UserPanelServices;
// using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdatePasswordService.Abstraction;
//
// namespace RelationshipAnalysis.Test.Services.Panel.UserPanelServices;
//
// public class UserPasswordServiceTests
// {
//     private readonly ApplicationDbContext _context;
//     private readonly IPasswordHasher _passwordHasher;
//     private readonly IPasswordVerifier _passwordVerifier;
//     private readonly IServiceProvider _serviceProvider;
//     private readonly IUserUpdatePasswordService _sut;
//
//     public UserPasswordServiceTests()
//     {
//         _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
//         _passwordVerifier = Substitute.For<IPasswordVerifier>();
//         _passwordHasher = Substitute.For<IPasswordHasher>();
//
//         var serviceCollection = new ServiceCollection();
//         serviceCollection.AddScoped(_ => _context);
//
//         _serviceProvider = serviceCollection.BuildServiceProvider();
//
//         _sut = new UserPasswordService(_serviceProvider, _passwordVerifier, _passwordHasher);
//         SeedDatabase();
//     }
//
//     private void SeedDatabase()
//     {
//         _context.Users.Add(new User
//         {
//             Id = 1,
//             Username = "",
//             Email = "",
//             FirstName = "",
//             LastName = "",
//             PasswordHash = "HashedPassword"
//         });
//         _context.SaveChanges();
//     }
//
//     [Fact]
//     public async Task UpdatePasswordAsync_ReturnsNotFound_WhenUserIsNull()
//     {
//         // Arrange
//         User user = null;
//         var passwordInfoDto = new UserPasswordInfoDto
//         {
//             OldPassword = "oldPassword",
//             NewPassword = "newPassword"
//         };
//
//         // Act
//         var result = await _sut.UpdatePasswordAsync(user, passwordInfoDto);
//
//         // Assert
//         Assert.Equal(StatusCodeType.NotFound, result.StatusCode);
//         Assert.Equal(Resources.UserNotFoundMessage, result.Data.Message);
//     }
//
//     [Fact]
//     public async Task UpdatePasswordAsync_ReturnsBadRequest_WhenOldPasswordIsIncorrect()
//     {
//         // Arrange
//         var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == 1);
//         var passwordInfoDto = new UserPasswordInfoDto
//         {
//             OldPassword = "wrongOldPassword",
//             NewPassword = "newPassword"
//         };
//
//         _passwordVerifier.VerifyPasswordHash(passwordInfoDto.OldPassword, user.PasswordHash).Returns(false);
//
//         // Act
//         var result = await _sut.UpdatePasswordAsync(user, passwordInfoDto);
//
//         // Assert
//         Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
//         Assert.Equal(Resources.WrongOldPasswordMessage, result.Data.Message);
//     }
//
//     [Fact]
//     public async Task UpdatePasswordAsync_ReturnsSuccess_WhenPasswordIsUpdated()
//     {
//         // Arrange
//         var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == 1);
//         var passwordInfoDto = new UserPasswordInfoDto
//         {
//             OldPassword = "oldPassword",
//             NewPassword = "newPassword"
//         };
//
//         _passwordVerifier.VerifyPasswordHash(passwordInfoDto.OldPassword, user.PasswordHash).Returns(true);
//         _passwordHasher.HashPassword(passwordInfoDto.NewPassword).Returns("newHashedPassword");
//
//         // Act
//         var result = await _sut.UpdatePasswordAsync(user, passwordInfoDto);
//
//         // Assert
//         Assert.Equal(StatusCodeType.Success, result.StatusCode);
//         Assert.Equal(Resources.SuccessfulUpdateUserMessage, result.Data.Message);
//         Assert.Equal("newHashedPassword", user.PasswordHash);
//     }
// }