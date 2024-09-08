using RelationshipAnalysis.Dto.Graph;

namespace RelationshipAnalysis.Services.GraphServices.Graph.Abstraction
{
    public interface IExpansionCategoriesValidator
    {
        Task<(bool isValid, GraphDto graphDto)> ValidateCategories(
            string sourceCategoryName,
            string targetCategoryName,
            string edgeCategoryName);
    }
}