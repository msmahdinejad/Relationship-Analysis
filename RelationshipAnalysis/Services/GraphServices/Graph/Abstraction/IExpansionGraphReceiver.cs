using RelationshipAnalysis.Dto.Graph;

namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface IExpansionGraphReceiver
{
    Task<GraphDto> GetExpansionGraph(int nodeId, string sourceCategoryName, string targetCategoryName,
        string edgeCategoryName);
}