using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.GraphServices.Node;
using RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

namespace RelationshipAnalysis.Test.Services.GraphServices.Node;

public class SingleNodeAdditionServiceTests
{
    private readonly IServiceProvider _serviceProvider;
    private INodeValueAdditionService _nodeValueAdditionService;
    private SingleNodeAdditionService _sut;

    public SingleNodeAdditionServiceTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.NodeCategories.Add(new NodeCategory
        {
            NodeCategoryId = 1,
            NodeCategoryName = "Account"
        });
        var existingNode = new Models.Graph.Node.Node { NodeUniqueString = "TestNode", NodeCategoryId = 1, NodeId = 1 };
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
    public async Task AddSingleNode_ShouldCallTheAddFunction_WhenValidRecordIsProvided()
    {
        // Arrange
        var record = new Dictionary<string, object>
        {
            { "UniqueName", "TestNode2" },
            { "Attribute1", "Value1" }
        };

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _nodeValueAdditionService = Substitute.For<INodeValueAdditionService>();

        _sut = new SingleNodeAdditionService(_nodeValueAdditionService);

        // Act
        await _sut.AddSingleNode(context, record, "UniqueName", 1);

        // Assert
        await _nodeValueAdditionService.Received().AddKvpToValues(Arg.Any<ApplicationDbContext>(),
            Arg.Any<KeyValuePair<string, object>>(), Arg.Any<Models.Graph.Node.Node>());
    }

    [Fact]
    public async Task AddSingleNode_ShouldCallTheAddFunction_WhenNodeExists()
    {
        // Arrange
        var record = new Dictionary<string, object>
        {
            { "UniqueName", "TestNode" },
            { "Attribute2", "Value2" }
        };

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        _nodeValueAdditionService = Substitute.For<INodeValueAdditionService>();

        _sut = new SingleNodeAdditionService(_nodeValueAdditionService);

        // Act
        await _sut.AddSingleNode(context, record, "UniqueName", 1);

        // Assert
        await _nodeValueAdditionService.Received().AddKvpToValues(Arg.Any<ApplicationDbContext>(),
            Arg.Any<KeyValuePair<string, object>>(), Arg.Any<Models.Graph.Node.Node>());
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

        _nodeValueAdditionService = Substitute.For<INodeValueAdditionService>();

        _nodeValueAdditionService.AddKvpToValues(Arg.Any<ApplicationDbContext>(),
            Arg.Any<KeyValuePair<string, object>>(), Arg.Any<Models.Graph.Node.Node>()).Throws(new Exception());

        _sut = new SingleNodeAdditionService(_nodeValueAdditionService);

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
            { "Attribute1", "ExistingValue" }
        };

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        _nodeValueAdditionService = Substitute.For<INodeValueAdditionService>();

        _nodeValueAdditionService.AddKvpToValues(Arg.Any<ApplicationDbContext>(),
            Arg.Any<KeyValuePair<string, object>>(), Arg.Any<Models.Graph.Node.Node>()).ThrowsAsync(new Exception());

        _sut = new SingleNodeAdditionService(_nodeValueAdditionService);


        // Act
        var action = () => _sut.AddSingleNode(context, record, "UniqueName", 1);

        // Assert
        await Assert.ThrowsAsync<Exception>(action);
    }
}