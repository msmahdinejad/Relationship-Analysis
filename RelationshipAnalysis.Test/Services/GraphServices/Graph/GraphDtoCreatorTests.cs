using RelationshipAnalysis.Services.GraphServices.Graph;

namespace RelationshipAnalysis.Test.Services.GraphServices.Graph;

public class GraphDtoCreatorTests
{
    private readonly GraphDtoCreator _graphDtoCreator;

    public GraphDtoCreatorTests()
    {
        _graphDtoCreator = new GraphDtoCreator();
    }

    [Fact]
    public void CreateResultGraphDto_ShouldReturnGraphDto_WithCorrectNodesAndEdges()
    {
        // Arrange
        var contextNodes = new List<Models.Graph.Node.Node>
        {
            new Models.Graph.Node.Node { NodeId = 1, NodeCategory = new Models.Graph.Node.NodeCategory { NodeCategoryName = "Category1" }, NodeUniqueString = "Unique1" },
            new Models.Graph.Node.Node { NodeId = 2, NodeCategory = new Models.Graph.Node.NodeCategory { NodeCategoryName = "Category2" }, NodeUniqueString = "Unique2" }
        };

        var contextEdges = new List<Models.Graph.Edge.Edge>
        {
            new Models.Graph.Edge.Edge { EdgeId = 1, EdgeSourceNodeId = 1, EdgeDestinationNodeId = 2 },
            new Models.Graph.Edge.Edge { EdgeId = 2, EdgeSourceNodeId = 2, EdgeDestinationNodeId = 1 }
        };

        // Act
        var result = _graphDtoCreator.CreateResultGraphDto(contextNodes, contextEdges);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Nodes.Count);
        Assert.Equal(2, result.Edges.Count);

        Assert.Equal("1", result.Nodes[0].id);
        Assert.Equal("Category1/Unique1", result.Nodes[0].label);
        
        Assert.Equal("2", result.Nodes[1].id);
        Assert.Equal("Category2/Unique2", result.Nodes[1].label);

        Assert.Equal("1", result.Edges[0].id);
        Assert.Equal("1", result.Edges[0].source);
        Assert.Equal("2", result.Edges[0].target);

        Assert.Equal("2", result.Edges[1].id);
        Assert.Equal("2", result.Edges[1].source);
        Assert.Equal("1", result.Edges[1].target);
    }
    
    [Fact]
    public void CreateResultGraphDto_ShouldThrowException_WhenContextNodesAndEdgesAreNull()
    {
        // Arrange
        List<Models.Graph.Node.Node> contextNodes = null;
        List<Models.Graph.Edge.Edge> contextEdges = null;

        // Act
        var action = () => _graphDtoCreator.CreateResultGraphDto(contextNodes, contextEdges); 

        // Assert
        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void CreateResultGraphDto_ShouldReturnEmptyGraphDto_WhenContextNodesAndEdgesAreEmpty()
    {
        // Arrange
        var contextNodes = new List<Models.Graph.Node.Node>();
        var contextEdges = new List<Models.Graph.Edge.Edge>();

        // Act
        var result = _graphDtoCreator.CreateResultGraphDto(contextNodes, contextEdges);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Nodes);
        Assert.Empty(result.Edges);
    }
}