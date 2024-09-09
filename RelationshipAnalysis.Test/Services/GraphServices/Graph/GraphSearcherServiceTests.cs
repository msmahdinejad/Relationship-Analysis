using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.GraphServices.Graph;
using Moq;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Dto.Graph.Node;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;

namespace RelationshipAnalysis.Test.Services.GraphServices.Graph
{
    public class GraphSearcherServiceTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Mock<IGraphDtoCreator> _mockGraphDtoCreator;
        private readonly Mock<IAttributesReceiver> _mockNodeCategoryReceiver;
        private readonly Mock<IAttributesReceiver> _mockEdgeCategoryReceiver;
        private readonly Mock<ISearchNodeValidator> _mockNodeValidator;
        private readonly Mock<ISearchEdgeValidator> _mockEdgeValidator;
        private readonly Mock<IClauseValidatorService> _mockClauseValidatorService;
        private readonly GraphSearcherService _sut;

        public GraphSearcherServiceTests()
        {
            var serviceCollection = new ServiceCollection();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

            _serviceProvider = serviceCollection.BuildServiceProvider();

            _mockGraphDtoCreator = new Mock<IGraphDtoCreator>();
            _mockNodeCategoryReceiver = new Mock<IAttributesReceiver>();
            _mockEdgeCategoryReceiver = new Mock<IAttributesReceiver>();
            _mockNodeValidator = new Mock<ISearchNodeValidator>();
            _mockEdgeValidator = new Mock<ISearchEdgeValidator>();
            _mockClauseValidatorService = new Mock<IClauseValidatorService>();

            _sut = new GraphSearcherService(
                _mockGraphDtoCreator.Object,
                _mockNodeCategoryReceiver.Object,
                _mockEdgeCategoryReceiver.Object,
                _mockNodeValidator.Object,
                _mockEdgeValidator.Object,
                _mockClauseValidatorService.Object
            );
        }

        [Fact]
        public async Task Search_ShouldReturnValidationError_WhenClauseValidationFails()
        {
            // Arrange
            var searchGraphDto = new SearchGraphDto
            {
                SourceCategoryName = "Category1",
                TargetCategoryName = "Category2",
                EdgeCategoryName = "EdgeCategory"
            };

            _mockClauseValidatorService
                .Setup(s => s.AreClausesValid(It.IsAny<SearchGraphDto>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<string>>()))
                .ReturnsAsync(new ActionResponse<GraphDto> { StatusCode = StatusCodeType.BadRequest });

            // Act
            var result = await _sut.Search(searchGraphDto);

            // Assert
            Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Search_ShouldReturnEmptyGraph_WhenNoValidNodesOrEdges()
        {
            // Arrange
            
            _mockGraphDtoCreator
                .Setup(gdc => gdc.CreateResultGraphDto(It.IsAny<List<Models.Graph.Node.Node>>(), It.IsAny<List<Models.Graph.Edge.Edge>>()))
                .Returns(new GraphDto
                {
                    Nodes = new List<NodeDto>(),
                    Edges = new List<EdgeDto>()
                });

            var searchGraphDto = new SearchGraphDto
            {
                SourceCategoryName = "Category1",
                TargetCategoryName = "Category2",
                EdgeCategoryName = "EdgeCategory",
                EdgeCategoryClauses = new Dictionary<string, string>(),
                SourceCategoryClauses = new Dictionary<string, string>(),
                TargetCategoryClauses = new Dictionary<string, string>()
            };

            _mockClauseValidatorService
                .Setup(s => s.AreClausesValid(It.IsAny<SearchGraphDto>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<string>>()))
                .ReturnsAsync(new ActionResponse<GraphDto> { StatusCode = StatusCodeType.Success });

            _mockNodeValidator
                .Setup(nv => nv.GetValidNodes(It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Models.Graph.Node.Node>());

            _mockEdgeValidator
                .Setup(ev => ev.GetValidEdges(It.IsAny<List<Models.Graph.Node.Node>>(), It.IsAny<List<Models.Graph.Node.Node>>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(new List<Models.Graph.Edge.Edge>());

            // Act
            var result = await _sut.Search(searchGraphDto);

            // Assert
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data.Nodes);
            Assert.Empty(result.Data.Edges);
        }

        [Fact]
        public async Task Search_ShouldReturnValidGraph_WhenValidNodesAndEdgesAreFound()
        {
            // Arrange
            var searchGraphDto = new SearchGraphDto
            {
                SourceCategoryName = "Category1",
                TargetCategoryName = "Category2",
                EdgeCategoryName = "EdgeCategory",
                EdgeCategoryClauses = new Dictionary<string, string>(),
                SourceCategoryClauses = new Dictionary<string, string>(),
                TargetCategoryClauses = new Dictionary<string, string>()
            };

            var sourceNodes = new List<Models.Graph.Node.Node>
            {
                new Models.Graph.Node.Node { NodeId = 1, NodeCategoryId = 1, NodeUniqueString = "Node1" }
            };

            var targetNodes = new List<Models.Graph.Node.Node>
            {
                new Models.Graph.Node.Node { NodeId = 2, NodeCategoryId = 2, NodeUniqueString = "Node2" }
            };

            var edges = new List<Models.Graph.Edge.Edge>
            {
                new Models.Graph.Edge.Edge { EdgeUniqueString = "Edge1" }
            };

            _mockClauseValidatorService
                .Setup(s => s.AreClausesValid(It.IsAny<SearchGraphDto>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<string>>()))
                .ReturnsAsync(new ActionResponse<GraphDto> { StatusCode = StatusCodeType.Success });

            _mockNodeValidator
                .Setup(nv => nv.GetValidNodes(It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()))
                .ReturnsAsync(sourceNodes);

            _mockEdgeValidator
                .Setup(ev => ev.GetValidEdges(sourceNodes, targetNodes, "EdgeCategory", It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(edges);

            _mockGraphDtoCreator
                .Setup(gdc => gdc.CreateResultGraphDto(It.IsAny<List<Models.Graph.Node.Node>>(), It.IsAny<List<Models.Graph.Edge.Edge>>()))
                .Returns(new GraphDto
                {
                    Nodes = new List<NodeDto>()
                    {
                        new NodeDto()
                        {
                            id = "1", label = "Category1/Node1"
                        },
                        new NodeDto()
                        {
                            id = "2", label = "Category2/Node2"
                        }
                    },
                    Edges = new List<EdgeDto>()
                    {
                        new EdgeDto()
                        {
                            id = "1",
                            source = "1",
                            target = "2"
                        }
                    }
                });

            // Act
            var result = await _sut.Search(searchGraphDto);

            // Assert
            Assert.Equal(StatusCodeType.Success, result.StatusCode);
            Assert.Equal(2, result.Data.Nodes.Count);
            Assert.Single(result.Data.Edges);
        }
    }
}
