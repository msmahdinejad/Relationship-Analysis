using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Node;

namespace RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

public interface ICreateNodeCategoryService
{
    Task<ActionResponse<MessageDto>> CreateNodeCategory(CreateNodeCategoryDto createNodeCategoryDto);
}