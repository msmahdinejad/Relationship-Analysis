namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface ISearchNodeValidator
{
    Task<List<Models.Graph.Node.Node>> GetValidNodes(Dictionary<string, string> clauses, string categoryName);
}