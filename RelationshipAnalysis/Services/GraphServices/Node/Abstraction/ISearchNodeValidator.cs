namespace RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

public interface ISearchNodeValidator
{
    Task<List<Models.Graph.Node.Node>> GetValidNodes(Dictionary<string, string> clauses, string categoryName);
}