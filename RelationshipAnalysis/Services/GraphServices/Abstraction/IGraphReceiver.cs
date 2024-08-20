
using RelationshipAnalysis.Dto.Graph;

namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface IGraphReceiver
{
    Task<GraphDto> GetGraph();
}