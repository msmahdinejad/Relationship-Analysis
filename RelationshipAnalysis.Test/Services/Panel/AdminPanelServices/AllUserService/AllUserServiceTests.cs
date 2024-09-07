using FluentAssertions;
using Moq;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService.Abstraction;

namespace RelationshipAnalysis.Test.Services.Panel.AdminPanelServices.AllUserService;

public class AllUserServiceTests
{
    private readonly Mock<IAllUserDtoCreator> _allUserDtoCreatorMock;
    private readonly RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService.AllUserService _service;
    private readonly Mock<IAllUserServiceValidator> _validatorMock;

    public AllUserServiceTests()
    {
        _validatorMock = new Mock<IAllUserServiceValidator>();
        _allUserDtoCreatorMock = new Mock<IAllUserDtoCreator>();
        _service = new RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService.AllUserService(
            _validatorMock.Object, _allUserDtoCreatorMock.Object);
    }

    [Fact]
    public async Task GetAllUser_ShouldReturnSuccess_WhenValidationSucceeds()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = 1, Username = "user1", Email = "user1@example.com" }
        };
        var getAllUsersDto = new GetAllUsersDto
        {
            Users =
            [
                new UserOutputInfoDto
                {
                    Id = 1,
                    Username = "user1",
                    Email = "user1@example.com"
                }
            ],
            AllUserCount = 1
        };

        _validatorMock.Setup(v => v.Validate(It.IsAny<List<User>>()))
            .ReturnsAsync(new ActionResponse<GetAllUsersDto> { StatusCode = StatusCodeType.Success });

        _allUserDtoCreatorMock.Setup(c => c.Create(It.IsAny<List<User>>()))
            .ReturnsAsync(getAllUsersDto);

        // Act
        var result = await _service.GetAllUser(users);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodeType.Success);
        result.Data.Should().BeEquivalentTo(getAllUsersDto);
    }

    [Fact]
    public async Task GetAllUser_ShouldReturnValidationError_WhenValidationFails()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = 1, Username = "user1", Email = "user1@example.com" }
        };

        _validatorMock.Setup(v => v.Validate(It.IsAny<List<User>>()))
            .ReturnsAsync(new ActionResponse<GetAllUsersDto> { StatusCode = StatusCodeType.NotFound });

        // Act
        var result = await _service.GetAllUser(users);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodeType.NotFound);
        result.Data.Should().BeNull();
    }
}