// using AutoMapper;
// using NSubstitute;
// using RelationshipAnalysis.Dto.Panel.User;
// using RelationshipAnalysis.Enums;
// using RelationshipAnalysis.Models.Auth;
// using RelationshipAnalysis.Services.CRUD.Role.Abstraction;
// using RelationshipAnalysis.Services.Panel.UserPanelServices;
// using RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService;
//
// namespace RelationshipAnalysis.Test.Services.Panel.UserPanelServices;
//
// public class UserInfoServiceTests
// {
//     private readonly IMapper _mapper;
//     private readonly IRoleReceiver _rolesReceiver;
//     private readonly UserInfoService _service;
//
//     public UserInfoServiceTests()
//     {
//         _rolesReceiver = Substitute.For<IRoleReceiver>();
//         _mapper = Substitute.For<IMapper>();
//         _service = new UserInfoService();
//     }
//
//     [Fact]
//     public async Task GetUserAsync_ReturnsNotFound_WhenUserIsNull()
//     {
//         // Arrange
//         User user = null;
//
//         // Act
//         var result = await _service.GetUser(user);
//
//         // Assert
//         Assert.Equal(StatusCodeType.NotFound, result.StatusCode);
//         Assert.Null(result.Data);
//     }
//
//     [Fact]
//     public async Task GetUserAsync_ReturnsSuccess_WhenUserIsNotNull_()
//     {
//         // Arrange
//         var user = new User { Username = "Admin" };
//         var resultData = new UserOutputInfoDto { Username = "Admin" };
//         var roles = new List<string> { "Admin", "User" };
//
//         _mapper.Map<UserOutputInfoDto>(user).Returns(resultData);
//
//         _rolesReceiver.ReceiveRoleNamesAsync(user.Id).Returns(roles);
//
//         // Act
//         var result = await _service.GetUser(user);
//
//         // Assert
//         Assert.Equal(StatusCodeType.Success, result.StatusCode);
//         Assert.NotNull(result.Data);
//         Assert.Equal(user.Username, resultData.Username);
//         Assert.Equal(roles, result.Data.Roles);
//     }
// }