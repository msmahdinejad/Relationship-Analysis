using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.GraphServices.Edge;

namespace RelationshipAnalysis.Test.Services.GraphServices.Edge;

public class CreateEdgeCategoryServiceTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly CreateEdgeCategoryService _sut;

    public CreateEdgeCategoryServiceTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new CreateEdgeCategoryService(_serviceProvider, new MessageResponseCreator());

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.EdgeCategories.Add(new EdgeCategory
        {
            EdgeCategoryName = "ExistName"
        });
        context.SaveChanges();
    }

    [Fact]
    public async Task CreateEdgeCategory_ShouldReturnBadRequest_WhenDtoIsNull()
    {
        // Arrange
        CreateEdgeCategoryDto dto = null;

        // Act
        var result = await _sut.CreateEdgeCategory(dto);

        // Assert
        Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        Assert.Equal(Resources.NullDtoErrorMessage, result.Data.Message);
    }

    [Fact]
    public async Task CreateEdgeCategory_ShouldReturnBadRequest_WhenCategoryNameIsNotUnique()
    {
        // Arrange
        var dto = new CreateEdgeCategoryDto
        {
            EdgeCategoryName = "ExistName"
        };

        // Act
        var result = await _sut.CreateEdgeCategory(dto);

        // Assert
        Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        Assert.Equal(Resources.NotUniqueCategoryNameErrorMessage, result.Data.Message);
    }

    [Fact]
    public async Task CreateEdgeCategory_ShouldReturnSuccess_WhenDtoIsOk()
    {
        // Arrange
        var dto = new CreateEdgeCategoryDto
        {
            EdgeCategoryName = "NotExistName"
        };
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        var result = await _sut.CreateEdgeCategory(dto);
        var categoty = context.EdgeCategories.SingleOrDefault(c => c.EdgeCategoryName == dto.EdgeCategoryName);

        // Assert
        Assert.Equal(StatusCodeType.Success, result.StatusCode);
        Assert.Equal(Resources.SuccessfulCreateCategory, result.Data.Message);
        Assert.NotNull(categoty);
    }
}