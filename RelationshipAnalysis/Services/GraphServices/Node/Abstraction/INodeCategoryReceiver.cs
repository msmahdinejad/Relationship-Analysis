namespace RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

public interface INodeCategoryReceiver
{
    Task<List<string>> GetAllNodeCategories();
}