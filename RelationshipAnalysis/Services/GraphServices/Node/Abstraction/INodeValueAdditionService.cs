using RelationshipAnalysis.Context;

namespace RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

public interface INodeValueAdditionService
{
    Task AddKvpToValues(ApplicationDbContext context, KeyValuePair<string, object> kvp, Models.Graph.Node.Node newNode);
}