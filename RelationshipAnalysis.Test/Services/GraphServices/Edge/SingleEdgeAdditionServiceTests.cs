using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.GraphServices.Edge;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

namespace RelationshipAnalysis.Test.Services.GraphServices.Edge;

public class SingleEdgeAdditionServiceTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SingleEdgeAdditionService _sut;
    private readonly IEdgeValueAdditionService _edgeValueAdditionService;

    public SingleEdgeAdditionServiceTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _edgeValueAdditionService = Substitute.For<IEdgeValueAdditionService>();
        _sut = new SingleEdgeAdditionService(_edgeValueAdditionService);
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
        context.Nodes.Add(new Models.Graph.Node.Node
        {
            NodeId = 1,
            NodeUniqueString = "acc1",
            NodeCategoryId = 1
        });
        context.Nodes.Add(new Models.Graph.Node.Node
        {
            NodeId = 2,
            NodeUniqueString = "acc2",
            NodeCategoryId = 1
        });
        context.EdgeCategories.Add(new EdgeCategory
        {
            EdgeCategoryId = 1,
            EdgeCategoryName = "Transaction"
        });
        context.Edges.Add(new Models.Graph.Edge.Edge
        {
            EdgeId = 1,
            EdgeUniqueString = "tran1",
            EdgeCategoryId = 1,
            EdgeSourceNodeId = 1,
            EdgeDestinationNodeId = 2
        });
        context.EdgeAttributes.Add(new EdgeAttribute
        {
            EdgeAttributeId = 1,
            EdgeAttributeName = "att1"
        });
        context.EdgeValues.Add(new EdgeValue
        {
            EdgeId = 1,
            EdgeAttributeId = 1,
            ValueId = 1,
            ValueData = "value1"
        });
        context.SaveChanges();
    }


    [Fact]
    public async Task AddSingleEdge_ShouldCallAddKvpToValues_WhenValidRecordIsProvided()
    {
        // Arrange
        var record = new Dictionary<string, object>
        {
            { "UniqueEdge", "TestEdge" },
            { "SourceNode", "acc1" },
            { "TargetNode", "acc2" },
            { "Attribute1", "Value1" }
        };

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        await _sut.AddSingleEdge(context, record, "UniqueEdge", "SourceNode", "TargetNode", 1, 1, 1);

        // Assert

        _edgeValueAdditionService.Received().AddKvpToValues(context, Arg.Any<KeyValuePair<string, object>>(),
            Arg.Any<Models.Graph.Edge.Edge>());
    }
    //
    // [Fact]
    // public async Task AddSingleEdge_ShouldCallAddKvpToValues_WhenEdgeExists()
    // {
    //     // Arrange
    //     var record = new Dictionary<string, object>
    //     {
    //         { "UniqueEdge", "tran1" },
    //         { "SourceNode", "acc1" },
    //         { "TargetNode", "acc2" },
    //         { "Attribute2", "Value2" }
    //     };
    //
    //     using var scope = _serviceProvider.CreateScope();
    //     var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //
    //
    //     // Act
    //     await _sut.AddSingleEdge(context, record, "UniqueEdge", "SourceNode", "TargetNode", 1, 1, 1);
    //
    //     // Assert
    //     var edge = await context.Edges.SingleOrDefaultAsync(e => e.EdgeUniqueString == "tran1");
    //     Assert.NotNull(edge);
    //     Assert.Equal(1, edge.EdgeSourceNodeId);
    //     Assert.Equal(2, edge.EdgeDestinationNodeId);
    //     Assert.Equal(1, edge.EdgeCategoryId);
    //
    //     var attribute = await context.EdgeAttributes.SingleOrDefaultAsync(a => a.EdgeAttributeName == "Attribute2");
    //     Assert.NotNull(attribute);
    //
    //     var edgeValue =
    //         await context.EdgeValues.SingleOrDefaultAsync(v => v.ValueData == "Value2" && v.EdgeId == edge.EdgeId);
    //     Assert.NotNull(edgeValue);
    // }

    [Fact]
    public async Task AddSingleEdge_ShouldThrowException_WhenUniqueEdgeNameIsEmpty()
    {
        // Arrange
        var record = new Dictionary<string, object>
        {
            { "UniqueEdge", "" },
            { "SourceNode", "acc1" },
            { "TargetNode", "acc2" },
            { "Attribute1", "Value1" }
        };

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


        // Act
        var action = () =>
            _sut.AddSingleEdge(context, record, "UniqueEdge", "SourceNode", "TargetNode", 1, 1, 1);

        // Assert
        await Assert.ThrowsAsync<Exception>(action);
    }

    [Fact]
    public async Task AddSingleEdge_ShouldThrowException_WhenUniqueTargetNodeNameIsEmpty()
    {
        // Arrange
        var record = new Dictionary<string, object>
        {
            { "UniqueEdge", "Test" },
            { "SourceNode", "" },
            { "TargetNode", "acc2" },
            { "Attribute1", "Value1" }
        };

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


        // Act
        var action = () =>
            _sut.AddSingleEdge(context, record, "UniqueEdge", "SourceNode", "TargetNode", 1, 1, 1);

        // Assert
        await Assert.ThrowsAsync<Exception>(action);
    }

    [Fact]
    public async Task AddSingleEdge_ShouldThrowException_WhenUniqueSourceNodeNameIsEmpty()
    {
        // Arrange
        var record = new Dictionary<string, object>
        {
            { "UniqueEdge", "Test" },
            { "SourceNode", "acc1" },
            { "TargetNode", "" },
            { "Attribute1", "Value1" }
        };

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        var action = () =>
            _sut.AddSingleEdge(context, record, "UniqueEdge", "SourceNode", "TargetNode", 1, 1, 1);

        // Assert
        await Assert.ThrowsAsync<Exception>(action);
    }

    [Fact]
    public async Task AddSingleEdge_ShouldThrowException_WhenSourceNodeDoesNotExist()
    {
        // Arrange
        var record = new Dictionary<string, object>
        {
            { "UniqueEdge", "TestEdge" },
            { "SourceNode", "NotExist" },
            { "TargetNode", "acc2" },
            { "Attribute1", "Value1" }
        };

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


        // Act
        var action = () =>
            _sut.AddSingleEdge(context, record, "UniqueEdge", "SourceNode", "TargetNode", 1, 1, 1);

        // Assert 
        await Assert.ThrowsAsync<Exception>(action);
    }

    [Fact]
    public async Task AddSingleEdge_ShouldThrowException_WhenTargetNodeDoesNotExist()
    {
        // Arrange
        var record = new Dictionary<string, object>
        {
            { "UniqueEdge", "TestEdge" },
            { "SourceNode", "acc1" },
            { "TargetNode", "NotExist" },
            { "Attribute1", "Value1" }
        };

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


        // Act
        var action = () =>
            _sut.AddSingleEdge(context, record, "UniqueEdge", "SourceNode", "TargetNode", 1, 1, 1);

        // Assert 
        await Assert.ThrowsAsync<Exception>(action);
    }

    //
    // [Fact]
    // public async Task AddSingleEdge_ShouldThrowException_WhenEdgeValueAlreadyExists()
    // {
    //     // Arrange
    //     var record = new Dictionary<string, object>
    //     {
    //         { "UniqueEdge", "tran1" },
    //         { "SourceNode", "acc1" },
    //         { "TargetNode", "acc2" },
    //         { "att1", "dsjsdnfukj" }
    //     };
    //
    //     using var scope = _serviceProvider.CreateScope();
    //     var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //
    //     // Act 
    //     var action = () =>
    //         _sut.AddSingleEdge(context, record, "UniqueEdge", "SourceNode", "TargetNode", 1, 1, 1);
    //
    //
    //     // Assert
    //     await Assert.ThrowsAsync<Exception>(action);
    // }
    //
    [Fact]
    public async Task AddSingleEdge_ShouldThrowException_WhenSourceOrDestinationIsDeiffrent()
    {
        // Arrange
        var record = new Dictionary<string, object>
        {
            { "UniqueEdge", "test" },
            { "SourceNode", "acc3" },
            { "TargetNode", "acc2" },
            { "att2", "dsjsdnfukj" }
        };

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


        // Act 
        var action = () =>
            _sut.AddSingleEdge(context, record, "UniqueEdge", "SourceNode", "TargetNode", 1, 1, 1);


        // Assert
        await Assert.ThrowsAsync<Exception>(action);
    }
}