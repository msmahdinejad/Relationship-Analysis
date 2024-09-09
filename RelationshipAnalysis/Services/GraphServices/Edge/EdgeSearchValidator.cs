using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Edge;

public class EdgeSearchValidator(IServiceProvider serviceProvider) : ISearchEdgeValidator
{
    public async Task<List<Models.Graph.Edge.Edge>> GetValidEdges(List<Models.Graph.Node.Node> sourceNodes,
        List<Models.Graph.Node.Node> targetNodes,
        string edgeCategory, Dictionary<string, string> clauses)
    {
        var sourceNodeIds = sourceNodes.Select(n => n.NodeId).ToList();
        var targetNodeIds = targetNodes.Select(n => n.NodeId).ToList();

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
        var edges = await context.Edges
            .Include(e => e.EdgeValues)
            .ThenInclude(ev => ev.EdgeAttribute)
            .Where(e => edgeCategory == e.EdgeCategory.EdgeCategoryName &&
                        sourceNodeIds.Contains(e.EdgeSourceNodeId) &&
                        targetNodeIds.Contains(e.EdgeDestinationNodeId))
            .ToListAsync();
            
        var validEdgesByClauses = edges.Where(e => AreEdgeAttributesValid(e, clauses)).ToList();
        return validEdgesByClauses;
    }
    
    private bool AreEdgeAttributesValid(Models.Graph.Edge.Edge edge, Dictionary<string, string> clauses)
    {
        var attributeValues = new Dictionary<string, string>();
        edge.EdgeValues.ToList().ForEach(ev => attributeValues.Add(ev.EdgeAttribute.EdgeAttributeName, ev.ValueData));

        foreach (var kvp in clauses)
        {
            var actualValue = attributeValues[kvp.Key];
            if (!actualValue.StartsWith(kvp.Value)) return false;
        }

        return true;
    }
}