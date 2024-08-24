using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Graph;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class SingleNodeAdditionService(IServiceProvider serviceProvider) : ISingleNodeAdditionService
{
    public async Task AddSingleNode(IDictionary<string, object> record, string uniqueHeaderName, int nodeCategoryId)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (((string)record[uniqueHeaderName]).IsNullOrEmpty())
        {
            throw new Exception(Resources.FailedAddRecordsMessage);
        }

        var newNode = await context.Nodes.SingleOrDefaultAsync(n =>
            n.NodeUniqueString == (string)record[uniqueHeaderName]
            && n.NodeCategoryId == nodeCategoryId);
        if (newNode == null)
        {
            newNode = new Node()
            {
                NodeUniqueString = (string)record[uniqueHeaderName],
                NodeCategoryId = nodeCategoryId,
            };
            
            await context.AddAsync(newNode);
            await context.SaveChangesAsync();
        } 

        foreach (var kvp in record)
        {
            if (kvp.Key != uniqueHeaderName)
            {
                var newNodeAttribute = await context.NodeAttributes.SingleOrDefaultAsync(na =>
                    na.NodeAttributeName == kvp.Key);
                if (newNodeAttribute == null)
                {
                    newNodeAttribute = new NodeAttribute()
                    {
                        NodeAttributeName = kvp.Key
                    };
                    await context.AddAsync(newNodeAttribute);
                    await context.SaveChangesAsync();
                }

                var value = await context.NodeValues.SingleOrDefaultAsync(nv =>
                    nv.NodeAttributeId == newNodeAttribute.NodeAttributeId &&
                    nv.NodeId == newNode.NodeId);

                if (value != null)
                {
                    throw new Exception(Resources.FailedAddRecordsMessage);
                }

                var newNodeValue = new NodeValue()
                {
                    NodeAttributeId = newNodeAttribute.NodeAttributeId,
                    ValueData = kvp.Value.ToString(),
                    NodeId = newNode.NodeId
                };

                await context.AddAsync(newNodeValue);
                await context.SaveChangesAsync();
            }
        }
    }
}