using System.Threading.Tasks;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Node;

namespace RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

public interface INodesAdditionService
{
    Task<ActionResponse<MessageDto>> AddNodes(UploadNodeDto uploadNodeDto);
}