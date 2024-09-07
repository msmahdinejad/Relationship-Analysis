using RelationshipAnalysis.Context;

namespace RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

public interface ISingleEdgeAdditionService
{
    Task AddSingleEdge(
        ApplicationDbContext context, IDictionary<string, object> record,
        string uniqueHeaderName,
        string uniqueSourceHeaderName,
        string uniqueTargetHeaderName, int edgeCategoryId, int sourceNodeCategoryId, int targetNodeCategoryId);
}