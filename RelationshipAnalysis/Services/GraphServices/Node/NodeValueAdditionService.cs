using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Node;

public class NodeValueAdditionService : INodeValueAdditionService
{
    public async Task AddKvpToValues(ApplicationDbContext context, KeyValuePair<string, object> kvp,
        Models.Graph.Node.Node newNode)
    {
        var newNodeAttribute = await GetNewNodeAttribute(context, kvp);

        var value = await GetNodeValue(context, newNode, newNodeAttribute);

        if (value != null) throw new Exception(Resources.FailedAddRecordsMessage);

        var newNodeValue = new NodeValue
        {
            NodeAttributeId = newNodeAttribute.NodeAttributeId,
            ValueData = kvp.Value.ToString(),
            NodeId = newNode.NodeId
        };

        await context.AddAsync(newNodeValue);
        // await context.SaveChangesAsync();
    }

    private async Task<NodeValue?> GetNodeValue(ApplicationDbContext context, Models.Graph.Node.Node newNode,
        NodeAttribute newNodeAttribute)
    {
        var value = await context.NodeValues.SingleOrDefaultAsync(nv =>
            nv.NodeAttributeId == newNodeAttribute.NodeAttributeId &&
            nv.NodeId == newNode.NodeId);
        return value;
    }

    private async Task<NodeAttribute> GetNewNodeAttribute(ApplicationDbContext context,
        KeyValuePair<string, object> kvp)
    {
        
        var newNodeAttribute = await context.NodeAttributes.FirstOrDefaultAsync(na => na.NodeAttributeName == kvp.Key);
        
        if (newNodeAttribute == null)
        {
            newNodeAttribute = new NodeAttribute
            {
                NodeAttributeId = ++context.LastNodeAttribute,
                NodeAttributeName = kvp.Key
            };
            await context.AddAsync(newNodeAttribute);
            // await context.SaveChangesAsync();
        }

        return newNodeAttribute;
    }
}