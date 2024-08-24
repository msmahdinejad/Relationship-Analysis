namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface ISingleEdgeAdditionService
{
    Task AddSingleEdge(IDictionary<string, object> record, string uniqueHeaderName, string uniqueSourceHeaderName, string uniqueTargetHeaderName,
        int edgeCategoryId, int sourceNodeCategoryId, int targetNodeCategoryId);
}