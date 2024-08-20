using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CategoryServices.EdgeCategory;

namespace RelationshipAnalysis.Test.Services.CategoryServices.EdgeCategory;

public class EdgeCategoryReceiverTests
{
    private readonly EdgeCategoryReceiver _sut;
    private readonly IServiceProvider _serviceProvider;

    private Models.Graph.EdgeCategory Edge1 = new ()
    {
        EdgeCategoryName = "Edge1"
    };
    private Models.Graph.EdgeCategory Edge2 = new ()
    {
        EdgeCategoryName = "Edge2"
    };
    
    public EdgeCategoryReceiverTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new(_serviceProvider);

    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.EdgeCategories.AddRange(new List<Models.Graph.EdgeCategory>()
        {
            Edge1, Edge2
        });
        context.SaveChanges();
    }

    [Fact]
    public async Task GetAllEdgeCategories_ShouldReturnEmptyList_WhenThereIsNoCategory()
    {
        // Arrange

        // Act
        var result = await _sut.GetAllEdgeCategories();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllEdgeCategories_ShouldReturnCorrectList_WhenThereIsSomeCategories()
    {
        // Arrange
        SeedDatabase();
        
        // Act
        var result = await _sut.GetAllEdgeCategories();

        // Assert
        Assert.Contains(Edge1.EdgeCategoryName, result);
        Assert.Contains(Edge2.EdgeCategoryName, result);
    }
}