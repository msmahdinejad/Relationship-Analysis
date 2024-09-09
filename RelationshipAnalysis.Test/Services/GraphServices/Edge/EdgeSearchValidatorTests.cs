using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Services.GraphServices.Edge;

namespace RelationshipAnalysis.Test.Services.GraphServices.Edge
{
    public class EdgeSearchValidatorTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly EdgeSearchValidator _sut;

        private readonly Models.Graph.Node.Node _sourceNode1 = new()
        {
            NodeId = 1,
            NodeCategoryId = 1,
            NodeUniqueString = "1"
        };

        private readonly Models.Graph.Node.Node _targetNode1 = new()
        {
            NodeId = 2,
            NodeCategoryId = 2,
            NodeUniqueString = "2"
        };

        private readonly EdgeCategory _edgeCategory1 = new() { EdgeCategoryId = 1, EdgeCategoryName = "Category1" };

        private readonly Models.Graph.Edge.Edge _edge1 = new()
        {
            EdgeUniqueString = "E",
            EdgeCategoryId = 1,
            EdgeSourceNodeId = 1,
            EdgeDestinationNodeId = 2,
            EdgeValues = new List<EdgeValue>
            {
                new() { EdgeAttribute = new EdgeAttribute { EdgeAttributeName = "Relationship" }, ValueData = "Friend" }
            }
        };

        public EdgeSearchValidatorTests()
        {
            var serviceCollection = new ServiceCollection();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseLazyLoadingProxies()
                .Options;

            serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

            _serviceProvider = serviceCollection.BuildServiceProvider();

            _sut = new EdgeSearchValidator(_serviceProvider);
        }

        private void SeedDatabase()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Nodes.AddRange(_sourceNode1, _targetNode1);
            context.EdgeCategories.Add(_edgeCategory1);
            context.Edges.Add(_edge1);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetValidEdges_ShouldReturnEmptyList_WhenNoEdgesMatch()
        {
            // Arrange
            SeedDatabase();
            var sourceNodes = new List<Models.Graph.Node.Node> { _sourceNode1 };
            var targetNodes = new List<Models.Graph.Node.Node> { _targetNode1 };
            var clauses = new Dictionary<string, string> { { "Relationship", "Colleague" } };

            // Act
            var result = await _sut.GetValidEdges(sourceNodes, targetNodes, "Category1", clauses);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetValidEdges_ShouldReturnEdges_WhenClauseMatches()
        {
            // Arrange
            SeedDatabase();
            var sourceNodes = new List<Models.Graph.Node.Node> { _sourceNode1 };
            var targetNodes = new List<Models.Graph.Node.Node> { _targetNode1 };
            var clauses = new Dictionary<string, string> { { "Relationship", "Friend" } }; // "Friend" exists in Edge1

            // Act
            var result = await _sut.GetValidEdges(sourceNodes, targetNodes, "Category1", clauses);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, edge => edge.EdgeValues.Any(ev => ev.ValueData == "Friend"));
        }

        [Fact]
        public async Task GetValidEdges_ShouldReturnEmptyList_WhenEdgeCategoryNameDoesNotMatch()
        {
            // Arrange
            SeedDatabase();
            var sourceNodes = new List<Models.Graph.Node.Node> { _sourceNode1 };
            var targetNodes = new List<Models.Graph.Node.Node> { _targetNode1 };
            var clauses = new Dictionary<string, string> { { "Relationship", "Friend" } };

            // Act
            var result = await _sut.GetValidEdges(sourceNodes, targetNodes, "Category2", clauses);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetValidEdges_ShouldReturnCorrectEdges_WhenMultipleClausesMatch()
        {
            // Arrange
            SeedDatabase();
            var sourceNodes = new List<Models.Graph.Node.Node> { _sourceNode1 };
            var targetNodes = new List<Models.Graph.Node.Node> { _targetNode1 };
            var clauses = new Dictionary<string, string>
            {
                { "Relationship", "Friend" }
            };

            // Act
            var result = await _sut.GetValidEdges(sourceNodes, targetNodes, "Category1", clauses);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, edge => edge.EdgeValues.Any(ev => ev.ValueData == "Friend"));
        }
    }
}
