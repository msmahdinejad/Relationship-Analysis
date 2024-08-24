using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Graph;
using RelationshipAnalysis.Services.GraphServices;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Test.Services;

public class SingleNodeAdditionServiceTests
{
    private readonly SingleNodeAdditionService _sut;
    private readonly IServiceProvider _serviceProvider;

    public SingleNodeAdditionServiceTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new(_serviceProvider);
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.NodeCategories.Add(new NodeCategory()
        {
            NodeCategoryId = 1,
            NodeCategoryName = "Account",
        });
        var existingNode = new Node { NodeUniqueString = "TestNode", NodeCategoryId = 1, NodeId = 1 };
        var existingAttribute = new NodeAttribute { NodeAttributeName = "Attribute1", NodeAttributeId = 1 };
        context.NodeAttributes.Add(existingAttribute);
        var existingNodeValue = new NodeValue
        {
            ValueId = 1,
            NodeId = 1,
            NodeAttributeId = 1,
            ValueData = "ExistingValue"
        };
        context.NodeValues.Add(existingNodeValue);
        context.Add(existingNode);
        context.SaveChanges();
    }


    [Fact]
    public async Task AddSingleNode_ShouldAddNewNode_WhenValidRecordIsProvided()
    {
        // Arrange
        var record = new Dictionary<string, object>
        {
            { "UniqueName", "TestNode2" },
            { "Attribute1", "Value1" }
        };

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        await _sut.AddSingleNode(context, record, "UniqueName", 1);

        // Assert
       
        var node = await context.Nodes.SingleOrDefaultAsync(n => n.NodeUniqueString == "TestNode2");
        Assert.NotNull(node);
        Assert.Equal(1, node.NodeCategoryId);

        var attribute = await context.NodeAttributes.SingleOrDefaultAsync(a => a.NodeAttributeName == "Attribute1");
        Assert.NotNull(attribute);

        var nodeValue =
            await context.NodeValues.SingleOrDefaultAsync(v => v.ValueData == "Value1" && v.NodeId == node.NodeId);
        Assert.NotNull(nodeValue);
    }

    [Fact]
    public async Task AddSingleNode_ShouldAddAttributes_WhenNodeExists()
    {
        // Arrange
        var record = new Dictionary<string, object>
        {
            { "UniqueName", "TestNode" },
            { "Attribute2", "Value2" }
        };

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        await _sut.AddSingleNode(context, record, "UniqueName", 1);

        // Assert
        
        var node = await context.Nodes.SingleOrDefaultAsync(n => n.NodeUniqueString == "TestNode");
        Assert.NotNull(node);
        Assert.Equal(1, node.NodeCategoryId);

        var attribute = await context.NodeAttributes.SingleOrDefaultAsync(a => a.NodeAttributeName == "Attribute2");
        Assert.NotNull(attribute);

        var nodeValue =
            await context.NodeValues.SingleOrDefaultAsync(v => v.ValueData == "Value2" && v.NodeId == node.NodeId);
        Assert.NotNull(nodeValue);
    }

    [Fact]
    public async Task AddSingleNode_ShouldThrowException_WhenUniqueNameIsEmpty()
    {
        // Arrange
        var record = new Dictionary<string, object>
        {
            { "UniqueName", "" }, // Empty unique name
            { "Attribute1", "Value1" }
        };

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        var action = () => _sut.AddSingleNode(context, record, "UniqueName", 1);

        // Assert
        await Assert.ThrowsAsync<Exception>(action);
    }

    [Fact]
    public async Task AddSingleNode_ShouldThrowException_WhenNodeValueAlreadyExists()
    {
        // Arrange
        var record = new Dictionary<string, object>
        {
            { "UniqueName", "TestNode" },
            { "Attribute1", "ExistingValue" },
        };

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        var action = () => _sut.AddSingleNode(context, record, "UniqueName", 1);

        // Assert
        await Assert.ThrowsAsync<Exception>(action);
    }
}