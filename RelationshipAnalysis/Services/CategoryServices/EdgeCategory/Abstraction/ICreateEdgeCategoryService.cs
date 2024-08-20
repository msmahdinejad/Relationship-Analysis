using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Category;

namespace RelationshipAnalysis.Services.CategoryServices.EdgeCategory.Abstraction;

public interface ICreateEdgeCategoryService
{
    Task<ActionResponse<MessageDto>> CreateEdgeCategory(CreateEdgeCategoryDto createEdgeCategoryDto);
}