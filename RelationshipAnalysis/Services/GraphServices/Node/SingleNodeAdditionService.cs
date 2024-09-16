using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Node;

public class SingleNodeAdditionService(INodeValueAdditionService nodeValueAdditionService) : ISingleNodeAdditionService
{
    public async Task AddSingleNode(ApplicationDbContext context, IDictionary<string, object> record,
        string uniqueHeaderName, int nodeCategoryId)
    {
        if (((string)record[uniqueHeaderName]).IsNullOrEmpty()) throw new Exception(Resources.FailedAddRecordsMessage);

        var newNode = await GetNewNode(context, record, uniqueHeaderName, nodeCategoryId);

        foreach (var kvp in record) await nodeValueAdditionService.AddKvpToValues(context, kvp, newNode);
    }

    private async Task<Models.Graph.Node.Node> GetNewNode(ApplicationDbContext context,
        IDictionary<string, object> record, string uniqueHeaderName,
        int nodeCategoryId)
    {
        var newNode = await context.Nodes.SingleOrDefaultAsync(n =>
            n.NodeUniqueString == (string)record[uniqueHeaderName]
            && n.NodeCategoryId == nodeCategoryId);

        if (newNode == null)
        {
            newNode = new Models.Graph.Node.Node
            {
                NodeId = context.LastNode + 1,
                NodeUniqueString = (string)record[uniqueHeaderName],
                NodeCategoryId = nodeCategoryId
            };


            await context.AddAsync(newNode);
            // await context.SaveChangesAsync();
        }

        return newNode;
    }
}