using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        
        var inputNodes = await GetInputNodes(context, sourceCategoryName);
        var outputNodes = await GetOutputNodes(context, targetCategoryName);
        var validEdges = await GetValidEdges(nodeId, context, edgeCategoryName, inputNodes, outputNodes);

        return graphDtoCreator.CreateResultGraphDto(inputNodes.Union(outputNodes).ToList(), validEdges);
    }

    private async Task<List<Models.Graph.Edge.Edge>> GetValidEdges(int nodeId, ApplicationDbContext context, string edgeCategoryName, List<Models.Graph.Node.Node> inputNodes,
        List<Models.Graph.Node.Node> outputNodes)
    {
        
        var validEdges = await context.Edges.Where(e =>
            (e.EdgeSourceNodeId == nodeId || e.EdgeDestinationNodeId == nodeId) &&
            e.EdgeCategory.EdgeCategoryName == edgeCategoryName &&
            inputNodes.Contains(e.NodeSource) &&
            outputNodes.Contains(e.NodeDestination)).ToListAsync();
        return validEdges;
    }

    private async Task<List<Models.Graph.Node.Node>> GetOutputNodes(ApplicationDbContext context, string targetCategoryName)
    {
        var outputNodes =
            await context.Nodes.Where(n => n.NodeCategory.NodeCategoryName == targetCategoryName).ToListAsync();
        return outputNodes;
    }

    private async Task<List<Models.Graph.Node.Node>> GetInputNodes(ApplicationDbContext context, string sourceCategoryName)
    {
        var inputNodes =
            await context.Nodes.Where(n => n.NodeCategory.NodeCategoryName == sourceCategoryName).ToListAsync();
        return inputNodes;
    }
}