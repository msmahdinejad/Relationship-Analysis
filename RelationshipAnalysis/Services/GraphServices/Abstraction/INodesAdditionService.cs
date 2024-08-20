using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;

namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface INodesAdditionService
{
    Task<ActionResponse<MessageDto>> AddNodes(UploadNodeDto uploadNodeDto);
}