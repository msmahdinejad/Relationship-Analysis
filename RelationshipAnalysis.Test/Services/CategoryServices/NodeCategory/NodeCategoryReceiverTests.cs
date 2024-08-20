using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CategoryServices.NodeCategory;

namespace RelationshipAnalysis.Test.Services.CategoryServices.NodeCategory;

public class NodeCategoryReceiverTests
{
    private readonly NodeCategoryReceiver _sut;
    private readonly IServiceProvider _serviceProvider;

    private Models.Graph.NodeCategory Node1 = new ()
    {
        NodeCategoryName = "Node1"
    };
    private Models.Graph.NodeCategory Node2 = new ()
    {
        NodeCategoryName = "Node2"
    };
    
    public NodeCategoryReceiverTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new NodeCategoryReceiver(_serviceProvider);

    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.NodeCategories.AddRange(new List<Models.Graph.NodeCategory>()
        {
            Node1, Node2
        });
        context.SaveChanges();
    }

    [Fact]
    public async Task GetAllNodeCategories_ShouldReturnEmptyList_WhenThereIsNoCategory()
    {
        // Arrange

        // Act
        var result = await _sut.GetAllNodeCategories();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllNodeCategories_ShouldReturnCorrectList_WhenThereIsSomeCategories()
    {
        // Arrange
        SeedDatabase();
        
        // Act
        var result = await _sut.GetAllNodeCategories();

        // Assert
        Assert.Contains(Node1.NodeCategoryName, result);
        Assert.Contains(Node2.NodeCategoryName, result);
    }
}