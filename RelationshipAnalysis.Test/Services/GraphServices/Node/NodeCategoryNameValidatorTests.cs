using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.GraphServices.Node;
using Xunit;

namespace RelationshipAnalysis.Test.Services.GraphServices.Node
{
    public class NodeCategoryNameValidatorTests
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly NodeCategoryNameValidator _sut;

        public NodeCategoryNameValidatorTests()
        {
            var serviceCollection = new ServiceCollection();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestNodeDb")
                .Options;

            serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

            _serviceProvider = serviceCollection.BuildServiceProvider();

            _sut = new NodeCategoryNameValidator(_serviceProvider);
        }

        private void SeedDatabase()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.NodeCategories.AddRange(new List<NodeCategory>
            {
                new NodeCategory { NodeCategoryName = "Category1" },
                new NodeCategory { NodeCategoryName = "Category2" }
            });
            context.SaveChanges();
        }

        [Fact]
        public async Task Validate_ShouldReturnTrue_WhenCategoryExists()
        {
            // Arrange
            SeedDatabase();
            var categoryName = "Category1";

            // Act
            var result = await _sut.Validate(categoryName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Validate_ShouldReturnFalse_WhenCategoryDoesNotExist()
        {
            // Arrange
            SeedDatabase();
            var categoryName = "NonExistentCategory";

            // Act
            var result = await _sut.Validate(categoryName);

            // Assert
            Assert.False(result);
        }
    }
}
