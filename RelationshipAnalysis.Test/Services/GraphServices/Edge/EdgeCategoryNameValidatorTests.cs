using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Services.GraphServices.Edge;
using Xunit;

namespace RelationshipAnalysis.Test.Services.GraphServices.Edge
{
    public class EdgeCategoryNameValidatorTests
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly EdgeCategoryNameValidator _sut;

        public EdgeCategoryNameValidatorTests()
        {
            var serviceCollection = new ServiceCollection();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestEdgeDb")
                .Options;

            serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

            _serviceProvider = serviceCollection.BuildServiceProvider();

            _sut = new EdgeCategoryNameValidator(_serviceProvider);
        }

        private void SeedDatabase()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.EdgeCategories.AddRange(new List<EdgeCategory>
            {
                new EdgeCategory { EdgeCategoryName = "EdgeCategory1" },
                new EdgeCategory { EdgeCategoryName = "EdgeCategory2" }
            });
            context.SaveChanges();
        }

        [Fact]
        public async Task Validate_ShouldReturnTrue_WhenCategoryExists()
        {
            // Arrange
            SeedDatabase();
            var categoryName = "EdgeCategory1";

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
            var categoryName = "NonExistentEdgeCategory";

            // Act
            var result = await _sut.Validate(categoryName);

            // Assert
            Assert.False(result);
        }
    }
}
