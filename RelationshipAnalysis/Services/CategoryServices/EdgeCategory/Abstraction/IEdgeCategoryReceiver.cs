namespace RelationshipAnalysis.Services.CategoryServices.EdgeCategory.Abstraction;

public interface IEdgeCategoryReceiver
{
    Task<List<string>> GetAllEdgeCategories();
}