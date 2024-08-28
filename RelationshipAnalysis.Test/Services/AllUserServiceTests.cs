// using Xunit;
// using Moq;
// using AutoMapper;
// using Microsoft.Extensions.DependencyInjection;
// using RelationshipAnalysis.Context;
// using RelationshipAnalysis.Dto;
// using RelationshipAnalysis.Enums;
// using RelationshipAnalysis.Models.Auth;
// using RelationshipAnalysis.Services.AdminPanelServices;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using RelationshipAnalysis.Services.AdminPanelServices.Abstraction;
//
// public class AllUserServiceTests
// {
//     private readonly Mock<IServiceProvider> _serviceProviderMock;
//     private readonly Mock<IMapper> _mapperMock;
//     private readonly Mock<IRoleReceiver> _rolesReceiverMock;
//     private readonly Mock<ApplicationDbContext> _contextMock;
//     private readonly AllUserService _allUserService;
//
//     public AllUserServiceTests()
//     {
//         _serviceProviderMock = new Mock<IServiceProvider>();
//         _mapperMock = new Mock<IMapper>();
//         _rolesReceiverMock = new Mock<IRoleReceiver>();
//         _contextMock = new Mock<ApplicationDbContext>();
//
//         var serviceScopeMock = new Mock<IServiceScope>();
//         var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
//
//         serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(serviceScopeMock.Object);
//         serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
//
//         _serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
//             .Returns(serviceScopeFactoryMock.Object);
//         _serviceProviderMock.Setup(x => x.GetService(typeof(ApplicationDbContext))).Returns(_contextMock.Object);
//
//         _allUserService =
//             new AllUserService(_serviceProviderMock.Object, _mapperMock.Object, _rolesReceiverMock.Object);
//     }
//     
//     [Fact]
//     public async Task ReceiveAllUserCount_ShouldReturnUserCount_WhenUsersExistInContext()
//     {
//         // Arrange
//         var users = new List<User> { new User(), new User() };
//         var usersDbSetMock = new Mock<DbSet<User>>();
//
//         _contextMock.Setup(x => x.Users).ReturnsDbSet(users);
//
//         // Act
//         var result = await _allUserService.ReceiveAllUserCount();
//
//         // Assert
//         Assert.Equal(users.Count, result);
//     }
//
//     
// }