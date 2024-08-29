using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.UserUpdateRolesService;

namespace RelationshipAnalysis.Test.Services.Panel.AdminPanelServices.UserUpdateRolesService;

public class UserUpdateRolesServiceValidatorTests
{
    private readonly Mock<IMessageResponseCreator> _mockMessageResponseCreator;
    private readonly ServiceProvider _serviceProvider;
    private readonly UserUpdateRolesServiceValidator _validator;

    public UserUpdateRolesServiceValidatorTests()
    {
        _mockMessageResponseCreator = new Mock<IMessageResponseCreator>();

        var serviceCollection = new ServiceCollection();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseLazyLoadingProxies()
            .Options;
        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _validator = new UserUpdateRolesServiceValidator(_serviceProvider, _mockMessageResponseCreator.Object);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Roles.AddRange(new List<Role>
        {
            new Role { Id = 1, Name = "Admin" , Permissions = ""},
            new Role { Id = 2, Name = "User" , Permissions = ""}
        });

        context.SaveChanges();
    }

    [Fact]
    public async Task Validate_EmptyRoles_ReturnsBadRequest()
    {
        // Arrange
        var user = new User();
        var emptyRoles = new List<string>();

        _mockMessageResponseCreator.Setup(m => m.Create(StatusCodeType.BadRequest, Resources.EmptyRolesMessage))
            .Returns(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.BadRequest,
                Data = new MessageDto(Resources.EmptyRolesMessage)
            });

        // Act
        var result = await _validator.Validate(user, emptyRoles);

        // Assert
        Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        Assert.Equal(Resources.EmptyRolesMessage, result.Data.Message);
    }

    [Fact]
    public async Task Validate_InvalidRoles_ReturnsBadRequest()
    {
        // Arrange
        var user = new User();
        var newRoles = new List<string> { "Admin", "InvalidRole" };

        _mockMessageResponseCreator.Setup(m => m.Create(StatusCodeType.BadRequest, Resources.InvalidRolesListMessage))
            .Returns(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.BadRequest,
                Data = new MessageDto(Resources.InvalidRolesListMessage)
            });

        // Act
        var result = await _validator.Validate(user, newRoles);

        // Assert
        Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        Assert.Equal(Resources.InvalidRolesListMessage, result.Data.Message);
    }

    [Fact]
    public async Task Validate_ValidRoles_ReturnsSuccess()
    {
        // Arrange
        var user = new User();
        var newRoles = new List<string> { "Admin", "User" };

        _mockMessageResponseCreator.Setup(m => m.Create(StatusCodeType.Success, Resources.SuccessfulUpdateRolesMessage))
            .Returns(new ActionResponse<MessageDto>
            {
                StatusCode = StatusCodeType.Success,
                Data = new MessageDto(Resources.SuccessfulUpdateRolesMessage)
            });

        // Act
        var result = await _validator.Validate(user, newRoles);

        // Assert
        Assert.Equal(StatusCodeType.Success, result.StatusCode);
        Assert.Equal(Resources.SuccessfulUpdateRolesMessage, result.Data.Message);
    }
}