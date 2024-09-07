using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.CreateUserService;

namespace RelationshipAnalysis.Test.Services.Panel.AdminPanelServices.CreateUserServiceTests;

public class UserCreateServiceValidatorTests
{
    private readonly Mock<IMessageResponseCreator> _messageResponseCreatorMock;
    private readonly IServiceProvider _serviceProvider;
    private readonly UserCreateServiceValidator _validator;

    public UserCreateServiceValidatorTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseLazyLoadingProxies()
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _messageResponseCreatorMock = new Mock<IMessageResponseCreator>();

        _validator = new UserCreateServiceValidator(_serviceProvider, _messageResponseCreatorMock.Object);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(new User
        {
            Id = 1, Username = "user1", Email = "user1@example.com", PasswordHash = "hash1", FirstName = "User",
            LastName = "One"
        });

        context.Roles.AddRange(new List<Role>
        {
            new() { Name = "Admin", Permissions = "" },
            new() { Name = "User", Permissions = "" }
        });

        context.SaveChanges();
    }

    [Fact]
    public async Task Validate_ShouldReturnUsernameExistsMessage_WhenUsernameExists()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "user1",
            Password = "P@ssw0rd123",
            FirstName = "John",
            LastName = "Doe",
            Email = "newemail@example.com",
            Roles = new List<string> { "Admin" }
        };

        _messageResponseCreatorMock
            .Setup(m => m.Create(StatusCodeType.BadRequest, Resources.UsernameExistsMessage))
            .Returns(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.BadRequest,
                Data = new MessageDto(Resources.UsernameExistsMessage)
            });

        // Act
        var result = await _validator.Validate(dto);

        // Assert
        Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        Assert.Equal(Resources.UsernameExistsMessage, result.Data.Message);
    }

    [Fact]
    public async Task Validate_ShouldReturnEmailExistsMessage_WhenEmailExists()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "newuser",
            Password = "P@ssw0rd123",
            FirstName = "John",
            LastName = "Doe",
            Email = "user1@example.com",
            Roles = new List<string> { "Admin" }
        };

        _messageResponseCreatorMock
            .Setup(m => m.Create(StatusCodeType.BadRequest, Resources.EmailExistsMessage))
            .Returns(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.BadRequest,
                Data = new MessageDto(Resources.EmailExistsMessage)
            });

        // Act
        var result = await _validator.Validate(dto);

        // Assert
        Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        Assert.Equal(Resources.EmailExistsMessage, result.Data.Message);
    }

    [Fact]
    public async Task Validate_ShouldReturnEmptyRolesMessage_WhenRolesAreEmpty()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "newuser",
            Password = "P@ssw0rd123",
            FirstName = "John",
            LastName = "Doe",
            Email = "newemail@example.com",
            Roles = new List<string>() // Empty list
        };

        _messageResponseCreatorMock
            .Setup(m => m.Create(StatusCodeType.BadRequest, Resources.EmptyRolesMessage))
            .Returns(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.BadRequest,
                Data = new MessageDto(Resources.EmptyRolesMessage)
            });

        // Act
        var result = await _validator.Validate(dto);

        // Assert
        Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        Assert.Equal(Resources.EmptyRolesMessage, result.Data.Message);
    }

    [Fact]
    public async Task Validate_ShouldReturnInvalidRolesListMessage_WhenRolesAreInvalid()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "newuser",
            Password = "P@ssw0rd123",
            FirstName = "John",
            LastName = "Doe",
            Email = "newemail@example.com",
            Roles = new List<string> { "Admin", "NonExistentRole" }
        };

        _messageResponseCreatorMock
            .Setup(m => m.Create(StatusCodeType.BadRequest, Resources.InvalidRolesListMessage))
            .Returns(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.BadRequest,
                Data = new MessageDto(Resources.InvalidRolesListMessage)
            });

        // Act
        var result = await _validator.Validate(dto);

        // Assert
        Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        Assert.Equal(Resources.InvalidRolesListMessage, result.Data.Message);
    }

    [Fact]
    public async Task Validate_ShouldReturnSuccess_WhenAllValidationPass()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "newuser",
            Password = "P@ssw0rd123",
            FirstName = "John",
            LastName = "Doe",
            Email = "newemail@example.com",
            Roles = new List<string> { "Admin" }
        };

        _messageResponseCreatorMock
            .Setup(m => m.Create(StatusCodeType.Success, Resources.SucceddfulCreateUser))
            .Returns(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.Success,
                Data = new MessageDto(Resources.SucceddfulCreateUser)
            });

        // Act
        var result = await _validator.Validate(dto);

        // Assert
        Assert.Equal(StatusCodeType.Success, result.StatusCode);
        Assert.Equal(Resources.SucceddfulCreateUser, result.Data.Message);
    }
}