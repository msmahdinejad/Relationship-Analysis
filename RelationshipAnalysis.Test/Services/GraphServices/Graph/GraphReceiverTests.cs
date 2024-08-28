using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Dto.Graph.Node;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.GraphServices.Graph;
using RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;

namespace RelationshipAnalysis.Test.Services.GraphServices.Graph;

public class GraphReceiverTests
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private GraphReceiver _sut;

    public GraphReceiverTests()
    {
        _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(_ => _context);

        _serviceProvider = serviceCollection.BuildServiceProvider();

    }

    [Fact]
    public async Task GetGraph_ShouldReturnGraph_WhenDatabaseHasData()
    {
        // Arrange
        var nodeCategory1 = new NodeCategory { NodeCategoryName = "Account" };
        var nodeCategory2 = new NodeCategory { NodeCategoryName = "Person" };

        var node1 = new Models.Graph.Node.Node
        {
            NodeUniqueString = "Node1",
            NodeCategory = nodeCategory1,
            NodeCategoryId = nodeCategory1.NodeCategoryId
        };

        var node2 = new Models.Graph.Node.Node
        {
            NodeUniqueString = "Node2",
            NodeCategory = nodeCategory2,
            NodeCategoryId = nodeCategory2.NodeCategoryId
        };

        var edgeCategory = new EdgeCategory { EdgeCategoryName = "Transaction" };


        _context.NodeCategories.Add(nodeCategory1);
        _context.NodeCategories.Add(nodeCategory2);
        _context.Nodes.Add(node1);
        _context.Nodes.Add(node2);
        _context.EdgeCategories.Add(edgeCategory);


        var edge = new Models.Graph.Edge.Edge
        {
            EdgeSourceNodeId = node1.NodeId,
            EdgeDestinationNodeId = node2.NodeId,
            EdgeCategory = edgeCategory,
            EdgeCategoryId = edgeCategory.EdgeCategoryId,
            EdgeUniqueString = "Edge1"
        };

        _context.Edges.Add(edge);

        await _context.SaveChangesAsync();


        var expectedNodes = new List<NodeDto>
        {
            new()
            {
                id = node1.NodeId.ToString(),
                label = $"{node1.NodeCategory.NodeCategoryName}/{node1.NodeUniqueString}"
            },
            new()
            {
                id = node2.NodeId.ToString(),
                label = $"{node2.NodeCategory.NodeCategoryName}/{node2.NodeUniqueString}"
            }
        };

        var expectedEdges = new List<EdgeDto>
        {
            new()
            {
                id = edge.EdgeId.ToString(),
                source = node1.NodeId.ToString(),
                target = node2.NodeId.ToString()
            }
        };

        
        var allNodes = await _context.Nodes.ToListAsync();
        var allEdges = await _context.Edges.ToListAsync();
        var graphDtoCreatorMock = Substitute.For<IGraphDtoCreator>();
        
        graphDtoCreatorMock.CreateResultGraphDto(Arg.Any<List<Models.Graph.Node.Node>>(), Arg.Any<List<Models.Graph.Edge.Edge>>())
            .Returns(new GraphDto { Edges = expectedEdges, Nodes = expectedNodes });
        
        // Act
        _sut = new GraphReceiver(_serviceProvider, graphDtoCreatorMock);
        var resultGraph = await _sut.GetGraph();

        // Assert
        Assert.Equivalent(expectedNodes, resultGraph.Nodes);
        Assert.Equivalent(expectedEdges, resultGraph.Edges);
    }
}