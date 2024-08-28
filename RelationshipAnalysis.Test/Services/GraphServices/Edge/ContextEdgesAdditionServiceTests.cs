using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.GraphServices.Edge;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

namespace RelationshipAnalysis.Test.Services.GraphServices.Edge;

public class ContextEdgesAdditionServiceTests
{
    private IContextEdgesAdditionService _sut;
    private readonly IServiceProvider _serviceProvider;
    public ContextEdgesAdditionServiceTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

    }
    
    
    [Fact]
    public async Task AddToContext_ShouldReturnBadRequestAndRollBack_WhenDbFailsToAddData()
    {
        // Arrange
        
        var additionServiceMock = new Mock<ISingleEdgeAdditionService>();
        
        additionServiceMock.Setup(s => s.AddSingleEdge(
            It.IsAny<ApplicationDbContext>(),
            It.IsAny<IDictionary<string, object>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>()
        )).ThrowsAsync(new Exception("Custom exception message"));
        
        _sut = new ContextEdgesAdditionService(new MessageResponseCreator(), additionServiceMock.Object);
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        
        var nodeCategory = new NodeCategory { NodeCategoryId = 1 };
        var edgeCategory = new EdgeCategory { EdgeCategoryId = 1 };
        var objects = new List<dynamic> { new Dictionary<string, object>() {
            { "UniqueEdge", "TestEdge" },
            { "SourceNode", "acc1" },
            { "TargetNode", "acc2" },
            { "Attribute1", "Value1" }
        } };
        // Act
        var result = await _sut.AddToContext(context, edgeCategory, nodeCategory, nodeCategory, objects, new UploadEdgeDto());
        // Assert
        Assert.Equal(0, context.Edges.Count());
        Assert.Equal("Custom exception message", result.Data.Message);
    }
}