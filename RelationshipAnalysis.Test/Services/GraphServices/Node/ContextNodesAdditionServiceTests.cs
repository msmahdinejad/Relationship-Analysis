using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NSubstitute;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.GraphServices.Node;
using RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

namespace RelationshipAnalysis.Test.Services.GraphServices.Node;

public class ContextNodesAdditionServiceTests
{
    private readonly IServiceProvider _serviceProvider;
    private IContextNodesAdditionService _sut;

    public ContextNodesAdditionServiceTests()
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
    public async Task AddToContext_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var nodeCategory = new NodeCategory { NodeCategoryId = 1 };
        var objects = new List<dynamic>
        {
            new Dictionary<string, object>
            {
                { "UniqueEdge", "TestEdge" },
                { "SourceNode", "acc1" },
                { "TargetNode", "acc2" },
                { "Attribute1", "Value1" }
            }
        };

        var singleNodeAdditionService = Substitute.For<ISingleNodeAdditionService>();

        _sut = new ContextNodesAdditionService(new MessageResponseCreator(), singleNodeAdditionService);
        // Act
        var result = await _sut.AddToContext("UniqueEdge", context, objects, nodeCategory);

        // Assert
        Assert.Equal(StatusCodeType.Success, result.StatusCode);
    }

    [Fact]
    public async Task AddToContext_ShouldReturnBadRequestAndRollBack_WhenDbFailsToAddData()
    {
        // Arrange

        var additionServiceMock = new Mock<ISingleNodeAdditionService>();
        additionServiceMock
            .Setup(service => service.AddSingleNode(
                It.IsAny<ApplicationDbContext>(),
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<string>(),
                It.IsAny<int>()
            ))
            .ThrowsAsync(new Exception("Custom exception message"));
        _sut = new ContextNodesAdditionService(new MessageResponseCreator(), additionServiceMock.Object);


        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var newNodeCategory = new NodeCategory
        {
            NodeCategoryId = 1,
            NodeCategoryName = "Account"
        };
        context.NodeCategories.Add(newNodeCategory);
        await context.SaveChangesAsync();
        // Act
        var result = await _sut.AddToContext("AccountID", context, new List<dynamic>
        {
            new Dictionary<string, object>
            {
                { "AccountID", "6534454617" },
                { "CardID", "6104335000000190" },
                { "IBAN", "IR120778801496000000198" }
            },
            new Dictionary<string, object>
            {
                { "AccountID", "6534454617" },
                { "CardID", "6037699000000020" },
                { "IBAN", "IR033880987114000000028" }
            }
        }, newNodeCategory);

        // Assert
        Assert.Equal(0, context.Nodes.Count());
        Assert.Equal("Custom exception message", result.Data.Message);
    }
}