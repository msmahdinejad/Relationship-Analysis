using System.Threading.Tasks;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Graph
{
    public class ExpansionCategoriesValidator(
        [FromKeyedServices("node")] ICategoryNameValidator nodeCategoryValidator,
        [FromKeyedServices("edge")] ICategoryNameValidator edgeCategoryValidator)
        : IExpansionCategoriesValidator
    {
        public async Task<(bool isValid, GraphDto graphDto)> ValidateCategories(
            string sourceCategoryName, 
            string targetCategoryName, 
            string edgeCategoryName)
        {
            if (!await nodeCategoryValidator.Validate(sourceCategoryName))
            {
                return (false, new GraphDto
                {
                    Message = Resources.InvalidSourceNodeCategory
                });
            }

            if (!await nodeCategoryValidator.Validate(targetCategoryName))
            {
                return (false, new GraphDto
                {
                    Message = Resources.InvalidTargetNodeCategory
                });
            }

            if (!await edgeCategoryValidator.Validate(edgeCategoryName))
            {
                return (false, new GraphDto
                {
                    Message = Resources.InvalidEdgeCategory
                });
            }

            return (true, null);
        }
    }
}