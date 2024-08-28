using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class EdgeInfoReceiver(IServiceProvider serviceProvider) : IInfoReceiver
{
    public Task<ActionResponse<IDictionary<string, string>>> GetInfo(int edgeId)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var result = new Dictionary<string, string>();
        var selectedEdge = context.Edges.SingleOrDefault(e => e.EdgeId == edgeId);
        if (selectedEdge == null)
        {
            return NotFoundResult();
        }
        selectedEdge.EdgeValues.ToList().ForEach(v => result.Add(v.EdgeAttribute.EdgeAttributeName, v.ValueData));
        return SuccessResult(result);
    }

    private async Task<ActionResponse<IDictionary<string, string>>> NotFoundResult()
    {
        return new ActionResponse<IDictionary<string, string>>()
        {
            StatusCode = StatusCodeType.NotFound
        };
    }
    
    private async Task<ActionResponse<IDictionary<string, string>>> SuccessResult(Dictionary<string, string> result)
    {
        return new ActionResponse<IDictionary<string, string>>()
        {
            StatusCode = StatusCodeType.Success,
            Data =  result
        };
    }
}