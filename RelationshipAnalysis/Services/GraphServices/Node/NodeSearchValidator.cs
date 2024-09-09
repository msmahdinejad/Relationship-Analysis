using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Node;

public class NodeSearchValidator(IServiceProvider serviceProvider) : ISearchNodeValidator
{
    public async Task<List<Models.Graph.Node.Node>> GetValidNodes(Dictionary<string, string> clauses, string categoryName)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var allNodes = await context.Nodes.ToListAsync();
        var validNodes = allNodes.Where(sn => IsNodeValid(sn, clauses, categoryName)).ToList();
        return validNodes;
    }
    
    private bool IsNodeValid(Models.Graph.Node.Node node, Dictionary<string, string> clauses, string categoryName)
    {
        if (node.NodeCategory.NodeCategoryName != categoryName) return false;

        var attributeValues = new Dictionary<string, string>();
        node.Values.ToList().ForEach(nv => attributeValues.Add(nv.NodeAttribute.NodeAttributeName, nv.ValueData));

        foreach (var kvp in clauses)
        {
            if (!attributeValues.TryGetValue(kvp.Key, out var actualValue) || !actualValue.StartsWith(kvp.Value))
                return false;
        }

        return true;
    }
}