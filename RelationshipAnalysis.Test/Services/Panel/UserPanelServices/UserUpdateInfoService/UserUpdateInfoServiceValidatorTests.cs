using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService;

namespace RelationshipAnalysis.Test.Services.Panel.UserPanelServices.UserUpdateInfoService;

public class UserUpdateInfoServiceValidatorTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly UserUpdateInfoServiceValidator _sut;
    private readonly Mock<IMessageResponseCreator> _messageResponseCreatorMock;

    private readonly User _user1 = new User
    {
        Id = 1,
        Username = "test",
        Email = "test@example.com",
        PasswordHash = "hashedpassword",
        FirstName = "Jane",
        LastName = "Doe"
    };

    private readonly User _user2 = new User
    {
        Id = 2,
        Username = "existinguser",
        Email = "existing@example.com",
        PasswordHash = "hashedpassword",
        FirstName = "Jane",
        LastName = "Doe"
    };

    public UserUpdateInfoServiceValidatorTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseLazyLoadingProxies()
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _messageResponseCreatorMock = new Mock<IMessageResponseCreator>();

        _sut = new UserUpdateInfoServiceValidator(_serviceProvider, _messageResponseCreatorMock.Object);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user1);
        context.Users.Add(_user2);
        context.SaveChanges();
    }

    [Fact]
    public async Task Validate_UserIsNull_ShouldReturnNotFoundResponse()
    {
        // Arrange
        User user = null;
        var userUpdateInfoDto = new UserUpdateInfoDto
        {
            Username = "newusername",
            Email = "newemail@example.com",
            FirstName = "New",
            LastName = "User"
        };

        _messageResponseCreatorMock
            .Setup(m => m.Create(StatusCodeType.NotFound, Resources.UserNotFoundMessage))
            .Returns(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.NotFound,
                Data = new MessageDto(Resources.UserNotFoundMessage)
            });

        // Act
        var result = await _sut.Validate(user, userUpdateInfoDto);

        // Assert
        result.StatusCode.Should().Be(StatusCodeType.NotFound);
        result.Data.Message.Should().Be(Resources.UserNotFoundMessage);
    }

    [Fact]
    public async Task Validate_UsernameNotUnique_ShouldReturnBadRequestResponse()
    {
        // Arrange
        var userUpdateInfoDto = new UserUpdateInfoDto
        {
            Username = "existinguser",
            Email = "newemail@example.com",
            FirstName = "New",
            LastName = "User"
        };

        _messageResponseCreatorMock
            .Setup(m => m.Create(StatusCodeType.BadRequest, Resources.UsernameExistsMessage))
            .Returns(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.BadRequest,
                Data = new MessageDto(Resources.UsernameExistsMessage)
            });

        // Act
        var result = await _sut.Validate(_user1, userUpdateInfoDto);

        // Assert
        result.StatusCode.Should().Be(StatusCodeType.BadRequest);
        result.Data.Message.Should().Be(Resources.UsernameExistsMessage);
    }

    [Fact]
    public async Task Validate_EmailNotUnique_ShouldReturnBadRequestResponse()
    {
        // Arrange
        var userUpdateInfoDto = new UserUpdateInfoDto
        {
            Username = "newusername",
            Email = "existing@example.com",
            FirstName = "New",
            LastName = "User"
        };

        _messageResponseCreatorMock
            .Setup(m => m.Create(StatusCodeType.BadRequest, Resources.EmailExistsMessage))
            .Returns(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.BadRequest,
                Data = new MessageDto(Resources.EmailExistsMessage)
            });

        // Act
        var result = await _sut.Validate(_user1, userUpdateInfoDto);

        // Assert
        result.StatusCode.Should().Be(StatusCodeType.BadRequest);
        result.Data.Message.Should().Be(Resources.EmailExistsMessage);
    }

    [Fact]
    public async Task Validate_ValidUserUpdateInfo_ShouldReturnSuccessResponse()
    {
        // Arrange
        var userUpdateInfoDto = new UserUpdateInfoDto
        {
            Username = "newusername",
            Email = "newemail@example.com",
            FirstName = "New",
            LastName = "User"
        };

        _messageResponseCreatorMock
            .Setup(m => m.Create(StatusCodeType.Success, Resources.SuccessfulUpdateUserMessage))
            .Returns(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.Success,
                Data = new MessageDto(Resources.SuccessfulUpdateUserMessage)
            });

        // Act
        var result = await _sut.Validate(_user1, userUpdateInfoDto);

        // Assert
        result.StatusCode.Should().Be(StatusCodeType.Success);
        result.Data.Message.Should().Be(Resources.SuccessfulUpdateUserMessage);
    }
}