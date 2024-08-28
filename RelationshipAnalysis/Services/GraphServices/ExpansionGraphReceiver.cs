using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Models.Graph;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class ExpansionGraphReceiver(IServiceProvider serviceProvider, IGraphDtoCreator graphDtoCreator) : IExpansionGraphReceiver
{
    public async Task<GraphDto> GetExpansionGraph(int nodeId, string sourceCategoryName, string targetCategoryName, string edgeCategoryName)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var inputNodes = await GetInputNodes(sourceCategoryName, context);
        var outputNodes = await GetOutputNodes(targetCategoryName, context);
        var validEdges = await GetValidEdges(edgeCategoryName, context, inputNodes, outputNodes);

        return graphDtoCreator.CreateResultGraphDto(inputNodes.Union(outputNodes).ToList(), validEdges);
    }

    private async Task<List<Models.Graph.Edge.Edge>> GetValidEdges(string edgeCategoryName, ApplicationDbContext context, List<Models.Graph.Node.Node> inputNodes,
        List<Models.Graph.Node.Node> outputNodes)
    {
        var validEdges = await context.Edges.Where(e =>
            e.EdgeCategory.EdgeCategoryName == edgeCategoryName &&
            inputNodes.Contains(e.NodeSource) &&
            outputNodes.Contains(e.NodeDestination)).ToListAsync();
        return validEdges;
    }

    private async Task<List<Models.Graph.Node.Node>> GetOutputNodes(string targetCategoryName, ApplicationDbContext context)
    {
        var outputNodes =
            await context.Nodes.Where(n => n.NodeCategory.NodeCategoryName == targetCategoryName).ToListAsync();
        return outputNodes;
    }

    private async Task<List<Models.Graph.Node.Node>> GetInputNodes(string sourceCategoryName, ApplicationDbContext context)
    {
        var inputNodes =
            await context.Nodes.Where(n => n.NodeCategory.NodeCategoryName == sourceCategoryName).ToListAsync();
        return inputNodes;
    }
}