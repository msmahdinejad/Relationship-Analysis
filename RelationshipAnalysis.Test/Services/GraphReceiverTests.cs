using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Models.Graph;
using RelationshipAnalysis.Services.GraphServices;

namespace RelationshipAnalysis.Test.Services;

public class GraphReceiverTests
{
    private readonly GraphReceiver _sut;
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public GraphReceiverTests()
    {
        
        _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(_ => _context);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new GraphReceiver(_serviceProvider);
    }
    
    [Fact]
    public async Task GetGraph_ShouldReturnGraph_WhenDatabaseHasData()
    {
        // Arrange
        var nodeCategory1 = new NodeCategory { NodeCategoryName = "Account" };
        var nodeCategory2 = new NodeCategory { NodeCategoryName = "Person" };

        var node1 = new Node
        {
            NodeUniqueString = "Node1",
            NodeCategory = nodeCategory1,
            NodeCategoryId = nodeCategory1.NodeCategoryId
        };

        var node2 = new Node
        {
            NodeUniqueString = "Node2",
            NodeCategory = nodeCategory2,
            NodeCategoryId = nodeCategory2.NodeCategoryId
        };

        var edgeCategory = new EdgeCategory { EdgeCategoryName = "Transaction"};


        _context.NodeCategories.Add(nodeCategory1);
        _context.NodeCategories.Add(nodeCategory2);
        _context.Nodes.Add(node1);
        _context.Nodes.Add(node2);
        _context.EdgeCategories.Add(edgeCategory);
        
        
        var edge = new Edge
        {
            EdgeSourceNodeId = node1.NodeId,
            EdgeDestinationNodeId = node2.NodeId,
            EdgeCategory = edgeCategory,
            EdgeCategoryId = edgeCategory.EdgeCategoryId,
            EdgeUniqueString = "Edge1"
        };
        
        _context.Edges.Add(edge);

        await _context.SaveChangesAsync();


        var expectedNodes = new List<NodeDto>()
        {
            new NodeDto()
            {
                id = node1.NodeId.ToString(),
                label = $"{node1.NodeCategory.NodeCategoryName}/{node1.NodeUniqueString}"
            },
            new NodeDto()
            {
                id = node2.NodeId.ToString(),
                label = $"{node2.NodeCategory.NodeCategoryName}/{node2.NodeUniqueString}"
            }
        };

        var expectedEdges = new List<EdgeDto>()
        {
            new EdgeDto()
            {
                id = edge.EdgeId.ToString(),
                source = node1.NodeId.ToString(),
                target = node2.NodeId.ToString()
            }
        };
        // Act
        var resultGraph = await _sut.GetGraph();

        // Assert
        Assert.Equivalent(expectedNodes, resultGraph.nodes);
        Assert.Equivalent(expectedEdges, resultGraph.edges);
    }

}