using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Category;

namespace RelationshipAnalysis.Services.CategoryServices.NodeCategory.Abstraction;

public interface ICreateNodeCategoryService
{
    Task<ActionResponse<MessageDto>> CreateNodeCategory(CreateNodeCategoryDto createNodeCategoryDto);
}