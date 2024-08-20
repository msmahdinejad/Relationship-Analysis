using AngleSharp.Dom;
using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class GraphReceiver(IServiceProvider serviceProvider): IGraphReceiver
{
    public async Task<GraphDto> GetGraph()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var contextNodes = await context.Nodes.ToListAsync();
        var contextEdges = await context.Edges.ToListAsync();
        
        var resultGraphDto = new GraphDto();
        contextNodes.ForEach(n => resultGraphDto.nodes.Add(new NodeDto()
        {
            id = n.NodeId.ToString(),
            label = $"{n.NodeCategory.NodeCategoryName}/{n.NodeUniqueString}"
        }));
        contextEdges.ForEach(e => resultGraphDto.edges.Add(new EdgeDto()
        {
            id = e.EdgeId.ToString(),
            source = e.EdgeSourceNodeId.ToString(),
            target = e.EdgeDestinationNodeId.ToString()
        }));
        return resultGraphDto;
    }
}