using RelationshipAnalysis.Context;

namespace RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

public interface IEdgeValueAdditionService
{
    Task AddKvpToValues(ApplicationDbContext context, KeyValuePair<string, object> kvp, Models.Graph.Edge.Edge newEdge);
}