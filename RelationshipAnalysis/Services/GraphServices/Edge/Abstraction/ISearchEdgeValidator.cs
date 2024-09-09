namespace RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

public interface ISearchEdgeValidator
{
    Task<List<Models.Graph.Edge.Edge>> GetValidEdges(List<Models.Graph.Node.Node> sourceNodes,
        List<Models.Graph.Node.Node> targetNodes,
        string edgeCategory, Dictionary<string, string> clauses);
}