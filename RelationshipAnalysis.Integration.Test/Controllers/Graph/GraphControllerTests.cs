using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Moq;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Dto.Graph.Node;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.AuthServices;
using RelationshipAnalysis.Settings.Authentication;

namespace RelationshipAnalysis.Integration.Test.Controllers.Graph;

public class GraphControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly JwtSettings _jwtSettings;

    public GraphControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _jwtSettings = new JwtSettings
        {
            Key = "kajbdiuhdqhpjQE89HBSDJIABFCIWSGF89GW3EJFBWEIUBCZNMXCJNLZDKNJKSNJKFBIGW3EASHHDUIASZGCUI",
            ExpireMinutes = 60
        };
    }

    [Fact]
    public async Task GetGraph_ShouldReturnGraph_WhenUserIsAuthorized()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/graph");
        Mock<IOptions<JwtSettings>> mockJwtSettings = new();
        mockJwtSettings.Setup(m => m.Value).Returns(_jwtSettings);
        var user = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "74b2c5bd3a8de69c8c7c643e8b5c49d6552dc636aeb0995aff6f01a1f661a979",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com",
            UserRoles = new List<UserRole> { new() { Role = new Role { Name = "admin" } } }
        };

        var token = new JwtTokenGenerator(mockJwtSettings.Object).GenerateJwtToken(user);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


        var nodeCategory1 = new NodeCategory
        {
            NodeCategoryId = 1,
            NodeCategoryName = "Account"
        };
        var nodeCategory2 = new NodeCategory
        {
            NodeCategoryId = 2,
            NodeCategoryName = "Person"
        };

        var node1 = new Node
        {
            NodeId = 1,
            NodeUniqueString = "Node1",
            NodeCategory = nodeCategory1,
            NodeCategoryId = nodeCategory1.NodeCategoryId
        };

        var node2 = new Node
        {
            NodeId = 2,
            NodeUniqueString = "Node2",
            NodeCategory = nodeCategory2,
            NodeCategoryId = nodeCategory2.NodeCategoryId
        };

        var edgeCategory = new EdgeCategory
        {
            EdgeCategoryId = 1,
            EdgeCategoryName = "Transaction"
        };


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

        var edge = new Edge
        {
            EdgeId = 1,
            EdgeSourceNodeId = node1.NodeId,
            EdgeDestinationNodeId = node2.NodeId,
            EdgeCategory = edgeCategory,
            EdgeCategoryId = edgeCategory.EdgeCategoryId,
            EdgeUniqueString = "Edge1"
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


        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<GraphDto>();
        Assert.Equivalent(responseData.Nodes, expectedNodes);
        Assert.Equivalent(responseData.Edges, expectedEdges);
    }
}