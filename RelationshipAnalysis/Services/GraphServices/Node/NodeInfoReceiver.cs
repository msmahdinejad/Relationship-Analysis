using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class NodeInfoReceiver(IServiceProvider serviceProvider) : IInfoReceiver
{
    public Task<ActionResponse<IDictionary<string, string>>> GetInfo(int nodeId)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var result = new Dictionary<string, string>();
        var selectedNode = context.Nodes.SingleOrDefault(n => n.NodeId == nodeId);
        if (selectedNode == null) return NotFoundResult();
        selectedNode.Values.ToList().ForEach(v => result.Add(v.NodeAttribute.NodeAttributeName, v.ValueData));
        return SuccessResult(result);
    }

    private async Task<ActionResponse<IDictionary<string, string>>> NotFoundResult()
    {
        return new ActionResponse<IDictionary<string, string>>
        {
            StatusCode = StatusCodeType.NotFound
        };
    }

    private async Task<ActionResponse<IDictionary<string, string>>> SuccessResult(Dictionary<string, string> result)
    {
        return new ActionResponse<IDictionary<string, string>>
        {
            StatusCode = StatusCodeType.Success,
            Data = result
        };
    }
}