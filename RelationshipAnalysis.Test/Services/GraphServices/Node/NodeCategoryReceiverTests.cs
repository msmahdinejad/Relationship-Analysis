using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.GraphServices.Node;

namespace RelationshipAnalysis.Test.Services.GraphServices.Node;

public class NodeCategoryReceiverTests
{
    private readonly NodeCategory _node1 = new()
    {
        NodeCategoryName = "Node1"
    };

    private readonly NodeCategory _node2 = new()
    {
        NodeCategoryName = "Node2"
    };

    private readonly IServiceProvider _serviceProvider;
    private readonly NodeCategoryReceiver _sut;

    public NodeCategoryReceiverTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new NodeCategoryReceiver(_serviceProvider);
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.NodeCategories.AddRange(new List<NodeCategory>
        {
            _node1, _node2
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
        Assert.Contains(_node1.NodeCategoryName, result);
        Assert.Contains(_node2.NodeCategoryName, result);
    }
}