using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;

namespace RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;

public interface IGraphSearcherService
{
    Task<ActionResponse<GraphDto>> Search(SearchGraphDto searchGraphDto);
}