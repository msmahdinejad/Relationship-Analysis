using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.GraphServices.Graph;
using Moq;
using RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;

namespace RelationshipAnalysis.Test.Services.GraphServices.Graph;

public class ExpansionGraphReceiverTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ExpansionGraphReceiver _sut;
    private readonly Mock<IGraphDtoCreator> _graphDtoCreatorMock;
    private readonly Mock<IExpansionCategoriesValidator> _expansionCategoriesValidatorMock;

    private readonly Models.Graph.Node.Node _node1;
    private readonly Models.Graph.Node.Node _node2;
    private readonly Models.Graph.Edge.Edge _edge1;
    private readonly NodeCategory _sourceCategory;
    private readonly NodeCategory _targetCategory;
    private readonly EdgeCategory _edgeCategory;

    public ExpansionGraphReceiverTests()
    {
        
        _sourceCategory = new() { NodeCategoryId = 1, NodeCategoryName = "SourceCategory" };
        _targetCategory = new() { NodeCategoryId = 2, NodeCategoryName = "TargetCategory" };
        _edgeCategory = new() { EdgeCategoryId = 2, EdgeCategoryName = "EdgeCategory" };
        _node1 = new() { NodeId = 1, NodeUniqueString = "1", NodeCategoryId = 1};
        _node2 = new() { NodeId = 2, NodeUniqueString = "2", NodeCategoryId = 2};
        _edge1 = new() { EdgeId = 1, EdgeUniqueString = "1", EdgeSourceNodeId = 1, EdgeDestinationNodeId = 2, EdgeCategoryId = 2 };
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseLazyLoadingProxies()
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _graphDtoCreatorMock = new Mock<IGraphDtoCreator>();
        _expansionCategoriesValidatorMock = new Mock<IExpansionCategoriesValidator>();

        _sut = new ExpansionGraphReceiver(_serviceProvider, _graphDtoCreatorMock.Object, _expansionCategoriesValidatorMock.Object);
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.EdgeCategories.AddRange(new List<EdgeCategory> { _edgeCategory });
        context.NodeCategories.AddRange(new List<NodeCategory> { _sourceCategory, _targetCategory });
        context.Nodes.AddRange(new List<Models.Graph.Node.Node> { _node1, _node2 });
        context.Edges.AddRange(new List<Models.Graph.Edge.Edge> { _edge1 });
        context.SaveChanges();
    }

    [Fact]
    public async Task GetExpansionGraph_ShouldReturnGraphDto_WhenCategoriesAreValid()
    {
        // Arrange
        SeedDatabase();
        _expansionCategoriesValidatorMock.Setup(v => v.ValidateCategories(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((true, new GraphDto()));

        _graphDtoCreatorMock.Setup(c => c.CreateResultGraphDto(It.IsAny<List<Models.Graph.Node.Node>>(), It.IsAny<List<Models.Graph.Edge.Edge>>()))
            .Returns(new GraphDto());

        // Act
        var result = await _sut.GetExpansionGraph(1, "SourceCategory", "TargetCategory", "EdgeCategory");

        // Assert
        Assert.NotNull(result);
        _expansionCategoriesValidatorMock.Verify(v => v.ValidateCategories(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _graphDtoCreatorMock.Verify(c => c.CreateResultGraphDto(It.IsAny<List<Models.Graph.Node.Node>>(), It.IsAny<List<Models.Graph.Edge.Edge>>()), Times.Once);
    }

    [Fact]
    public async Task GetExpansionGraph_ShouldReturnInvalidGraphDto_WhenCategoriesAreInvalid()
    {
        // Arrange
        _expansionCategoriesValidatorMock.Setup(v => v.ValidateCategories(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((false, new GraphDto()));

        // Act
        var result = await _sut.GetExpansionGraph(1, "InvalidSourceCategory", "InvalidTargetCategory", "InvalidEdgeCategory");

        // Assert
        Assert.NotNull(result);
        _expansionCategoriesValidatorMock.Verify(v => v.ValidateCategories(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _graphDtoCreatorMock.Verify(c => c.CreateResultGraphDto(It.IsAny<List<Models.Graph.Node.Node>>(), It.IsAny<List<Models.Graph.Edge.Edge>>()), Times.Never);
    }

    [Fact]
    public async Task GetExpansionGraph_ShouldReturnEmptyGraphDto_WhenThereAreNoValidEdges()
    {
        // Arrange
        SeedDatabase();
        _expansionCategoriesValidatorMock.Setup(v => v.ValidateCategories(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((true, new GraphDto()));

        _graphDtoCreatorMock.Setup(c => c.CreateResultGraphDto(It.IsAny<List<Models.Graph.Node.Node>>(), It.IsAny<List<Models.Graph.Edge.Edge>>()))
            .Returns(new GraphDto());

        // Act
        var result = await _sut.GetExpansionGraph(999, "SourceCategory", "TargetCategory", "EdgeCategory");

        // Assert
        Assert.NotNull(result);
        _graphDtoCreatorMock.Verify(c => c.CreateResultGraphDto(It.Is<List<Models.Graph.Node.Node>>(nodes => nodes.Count == 0), It.Is<List<Models.Graph.Edge.Edge>>(edges => edges.Count == 0)), Times.Once);
    }
}
