using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;

namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface IEdgesAdditionService
{   
    Task<ActionResponse<MessageDto>> AddEdges(UploadEdgeDto uploadEdgeDto);
}