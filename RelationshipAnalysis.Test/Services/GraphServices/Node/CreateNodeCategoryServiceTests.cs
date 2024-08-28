using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto.Graph.Node;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.GraphServices.Node;

namespace RelationshipAnalysis.Test.Services.GraphServices.Node;

public class CreateNodeCategoryServiceTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly CreateNodeCategoryService _sut;

    public CreateNodeCategoryServiceTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new CreateNodeCategoryService(_serviceProvider, new MessageResponseCreator());

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.NodeCategories.Add(new NodeCategory
        {
            NodeCategoryName = "ExistName"
        });
        context.SaveChanges();
    }

    [Fact]
    public async Task CreateNodeCategory_ShouldReturnBadRequest_WhenDtoIsNull()
    {
        // Arrange
        CreateNodeCategoryDto dto = null;

        // Act
        var result = await _sut.CreateNodeCategory(dto);

        // Assert
        Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        Assert.Equal(Resources.NullDtoErrorMessage, result.Data.Message);
    }

    [Fact]
    public async Task CreateNodeCategory_ShouldReturnBadRequest_WhenCategoryNameIsNotUnique()
    {
        // Arrange
        var dto = new CreateNodeCategoryDto
        {
            NodeCategoryName = "ExistName"
        };

        // Act
        var result = await _sut.CreateNodeCategory(dto);

        // Assert
        Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        Assert.Equal(Resources.NotUniqueCategoryNameErrorMessage, result.Data.Message);
    }

    [Fact]
    public async Task CreateNodeCategory_ShouldReturnSuccess_WhenDtoIsOk()
    {
        // Arrange
        var dto = new CreateNodeCategoryDto
        {
            NodeCategoryName = "NotExistName"
        };
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        var result = await _sut.CreateNodeCategory(dto);
        var categoty = context.NodeCategories.SingleOrDefault(c => c.NodeCategoryName == dto.NodeCategoryName);

        // Assert
        Assert.Equal(StatusCodeType.Success, result.StatusCode);
        Assert.Equal(Resources.SuccessfulCreateCategory, result.Data.Message);
        Assert.NotNull(categoty);
    }
}