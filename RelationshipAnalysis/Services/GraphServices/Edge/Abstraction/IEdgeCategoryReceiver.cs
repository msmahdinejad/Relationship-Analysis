namespace RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

public interface IEdgeCategoryReceiver
{
    Task<List<string>> GetAllEdgeCategories();
}