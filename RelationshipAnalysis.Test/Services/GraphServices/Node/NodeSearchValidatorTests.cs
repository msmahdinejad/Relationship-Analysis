using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.GraphServices.Node;
using Xunit;

namespace RelationshipAnalysis.Test.Services.GraphServices.Node
{
    public class NodeSearchValidatorTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly NodeSearchValidator _sut;

        private readonly NodeCategory _nodeCategory1 = new() { NodeCategoryId = 1, NodeCategoryName = "Category1" };
        private readonly NodeCategory _nodeCategory2 = new() { NodeCategoryId = 2, NodeCategoryName = "Category2" };

        private readonly Models.Graph.Node.Node _node1 = new()
        {
            NodeCategoryId = 1,
            NodeUniqueString = "1",
            Values = new List<NodeValue>
            {
                new() { NodeAttribute = new NodeAttribute { NodeAttributeName = "Name" }, ValueData = "Alice" },
                new() { NodeAttribute = new NodeAttribute { NodeAttributeName = "Age" }, ValueData = "25" }
            }
        };

        private readonly Models.Graph.Node.Node _node2 = new()
        {
            NodeCategoryId = 2,
            NodeUniqueString = "2",
            Values = new List<NodeValue>
            {
                new() { NodeAttribute = new NodeAttribute { NodeAttributeName = "Name" }, ValueData = "Bob" },
                new() { NodeAttribute = new NodeAttribute { NodeAttributeName = "Age" }, ValueData = "30" }
            }
        };

        public NodeSearchValidatorTests()
        {
            var serviceCollection = new ServiceCollection();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseLazyLoadingProxies()
                .Options;

            serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

            _serviceProvider = serviceCollection.BuildServiceProvider();

            _sut = new NodeSearchValidator(_serviceProvider);
        }

        private void SeedDatabase()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.NodeCategories.AddRange(_nodeCategory1, _nodeCategory2);
            context.Nodes.AddRange(_node1, _node2);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetValidNodes_ShouldReturnEmptyList_WhenNoNodesMatch()
        {
            // Arrange
            SeedDatabase();
            var clauses = new Dictionary<string, string> { { "Name", "Charlie" } };

            // Act
            var result = await _sut.GetValidNodes(clauses, "Category1");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetValidNodes_ShouldReturnNodes_WhenClauseMatches()
        {
            // Arrange
            SeedDatabase();
            var clauses = new Dictionary<string, string> { { "Name", "Alice" } }; // "Alice" exists in Category1

            // Act
            var result = await _sut.GetValidNodes(clauses, "Category1");

            // Assert
            Assert.Single(result);
            Assert.Contains(result, node => node.Values.Any(v => v.ValueData == "Alice"));
        }

        [Fact]
        public async Task GetValidNodes_ShouldReturnEmptyList_WhenCategoryNameDoesNotMatch()
        {
            // Arrange
            SeedDatabase();
            var clauses = new Dictionary<string, string> { { "Name", "Alice" } };

            // Act
            var result = await _sut.GetValidNodes(clauses, "Category2");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetValidNodes_ShouldReturnCorrectNodes_WhenMultipleClausesMatch()
        {
            // Arrange
            SeedDatabase();
            var clauses = new Dictionary<string, string>
            {
                { "Name", "Alice" },
                { "Age", "25" }
            };

            // Act
            var result = await _sut.GetValidNodes(clauses, "Category1");

            // Assert
            Assert.Single(result);
            Assert.Contains(result, node => node.Values.Any(v => v.ValueData == "Alice") && node.Values.Any(v => v.ValueData == "25"));
        }
    }
}
