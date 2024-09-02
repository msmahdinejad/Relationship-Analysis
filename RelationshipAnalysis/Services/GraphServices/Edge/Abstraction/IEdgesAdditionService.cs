using System.Threading.Tasks;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Edge;

namespace RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

public interface IEdgesAdditionService
{
    Task<ActionResponse<MessageDto>> AddEdges(UploadEdgeDto uploadEdgeDto);
}