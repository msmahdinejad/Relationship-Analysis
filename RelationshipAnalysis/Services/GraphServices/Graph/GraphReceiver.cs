using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;
using IGraphDtoCreator = RelationshipAnalysis.Services.GraphServices.Graph.Abstraction.IGraphDtoCreator;

namespace RelationshipAnalysis.Services.GraphServices.Graph;

public class GraphReceiver(IServiceProvider serviceProvider, IGraphDtoCreator graphDtoCreator) : IGraphReceiver
{
    public async Task<GraphDto> GetGraph()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var contextNodes = await context.Nodes.Include(node => node.NodeCategory).ToListAsync();
        var contextEdges = await context.Edges.ToListAsync();

        return graphDtoCreator.CreateResultGraphDto(contextNodes, contextEdges);
    }

}