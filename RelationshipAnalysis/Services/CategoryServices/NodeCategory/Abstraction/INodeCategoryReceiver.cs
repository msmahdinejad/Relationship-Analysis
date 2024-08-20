namespace RelationshipAnalysis.Services.CategoryServices.NodeCategory.Abstraction;

public interface INodeCategoryReceiver
{
    Task<List<string>> GetAllNodeCategories();
}