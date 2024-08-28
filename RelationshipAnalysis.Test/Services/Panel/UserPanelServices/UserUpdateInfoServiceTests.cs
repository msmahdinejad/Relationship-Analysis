// using AutoMapper;
// using Microsoft.AspNetCore.Http;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Moq;
// using RelationshipAnalysis.Context;
// using RelationshipAnalysis.Dto.Panel.User;
// using RelationshipAnalysis.Enums;
// using RelationshipAnalysis.Models.Auth;
// using RelationshipAnalysis.Services.Panel.UserPanelServices;
//
// namespace RelationshipAnalysis.Test.Services.Panel.UserPanelServices;
//
// public class UserUpdateInfoServiceTests
// {
//     private readonly IServiceProvider _serviceProvider;
//     private readonly UserUpdateInfoService _sut;
//
//     private readonly User _user = new()
//     {
//         Id = 1,
//         Username = "existinguser",
//         Email = "existing@example.com",
//         PasswordHash = "hashedpassword",
//         FirstName = "John",
//         LastName = "Doe"
//     };
//
//     public UserUpdateInfoServiceTests()
//     {
//         var serviceCollection = new ServiceCollection();
//
//         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseInMemoryDatabase(Guid.NewGuid().ToString())
//             .Options;
//
//         serviceCollection.AddScoped(_ => new ApplicationDbContext(options));
//         serviceCollection.AddAutoMapper(typeof(UserUpdateInfoService));
//
//         _serviceProvider = serviceCollection.BuildServiceProvider();
//
//         var mapper = _serviceProvider.GetRequiredService<IMapper>();I
//         _sut = new UserUpdateInfoService(_serviceProvider, mapper);
//
//         SeedDatabase();
//     }
//
//     private void SeedDatabase()
//     {
//         using (var scope = _serviceProvider.CreateScope())
//         {
//             var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//             context.Users.AddRange(new List<User>
//             {
//                 _user,
//                 new()
//                 {
//                     Id = 2,
//                     Username = "existinguser2",
//                     Email = "existing2@example.com",
//                     PasswordHash = "hashedpassword",
//                     FirstName = "John",
//                     LastName = "Doe"
//                 }
//             });
//             context.SaveChanges();
//         }
//     }
//
//     [Fact]
//     public async Task UpdateUserAsync_ShouldReturnNotFound_WhenUserIsNull()
//     {
//         // Arrange
//         User user = null;
//         var dto = new UserUpdateInfoDto
//         {
//             Username = "newuser",
//             Email = "new@example.com"
//         };
//         var response = new Mock<HttpResponse>().Object;
//
//         // Act
//         var result = await _sut.UpdateUserAsync(user, dto, response);
//
//         // Assert
//         Assert.Equal(StatusCodeType.NotFound, result.StatusCode);
//         Assert.Equal(Resources.UserNotFoundMessage, result.Data.Message);
//     }
//
//     [Fact]
//     public async Task UpdateUserAsync_ShouldReturnBadRequest_WhenUsernameExists()
//     {
//         // Arrange
//         var dto = new UserUpdateInfoDto
//         {
//             Username = "existinguser2"
//         };
//         var response = new Mock<HttpResponse>().Object;
//
//         // Act
//         var result = await _sut.UpdateUserAsync(_user, dto, response);
//
//         // Assert
//         Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
//         Assert.Equal(Resources.UsernameExistsMessage, result.Data.Message);
//     }
//
//     [Fact]
//     public async Task UpdateUserAsync_ShouldReturnBadRequest_WhenEmailExists()
//     {
//         // Arrange
//         var dto = new UserUpdateInfoDto
//         {
//             Email = "existing2@example.com"
//         };
//         var response = new Mock<HttpResponse>().Object;
//
//         // Act
//         var result = await _sut.UpdateUserAsync(_user, dto, response);
//
//         // Assert
//         Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
//         Assert.Equal(Resources.EmailExistsMessage, result.Data.Message);
//     }
//
//     [Fact]
//     public async Task UpdateUserAsync_ShouldUpdateUserAndReturnSuccess_WhenValidData()
//     {
//         // Arrange
//         var dto = new UserUpdateInfoDto
//         {
//             Username = "newuser",
//             Email = "new@example.com",
//             FirstName = "Jane",
//             LastName = "Smith"
//         };
//         var response = new Mock<HttpResponse>().Object;
//
//         // Act
//         var result = await _sut.UpdateUserAsync(_user, dto, response);
//
//         // Assert
//         Assert.Equal(StatusCodeType.Success, result.StatusCode);
//         Assert.Equal(Resources.SuccessfulUpdateUserMessage, result.Data.Message);
//
//         using (var scope = _serviceProvider.CreateScope())
//         {
//             var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//             var updatedUser = context.Users.SingleOrDefault(u => u.Id == _user.Id);
//             Assert.NotNull(updatedUser);
//             Assert.Equal("newuser", updatedUser.Username);
//             Assert.Equal("new@example.com", updatedUser.Email);
//             Assert.Equal("Jane", updatedUser.FirstName);
//             Assert.Equal("Smith", updatedUser.LastName);
//         }
//     }
// }