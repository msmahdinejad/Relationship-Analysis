using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Services.GraphServices.Edge;

namespace RelationshipAnalysis.Test.Services.GraphServices.Edge;

public class EdgeCategoryReceiverTests
{
    private readonly EdgeCategory _edge1 = new()
    {
        EdgeCategoryName = "Edge1"
    };

    private readonly EdgeCategory _edge2 = new()
    {
        EdgeCategoryName = "Edge2"
    };

    private readonly IServiceProvider _serviceProvider;
    private readonly EdgeCategoryReceiver _sut;

    public EdgeCategoryReceiverTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new EdgeCategoryReceiver(_serviceProvider);
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.EdgeCategories.AddRange(new List<EdgeCategory>
        {
            _edge1, _edge2
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
        Assert.Contains(_edge1.EdgeCategoryName, result);
        Assert.Contains(_edge2.EdgeCategoryName, result);
    }
}