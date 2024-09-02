using System.Threading.Tasks;
using RelationshipAnalysis.Dto.Graph;

namespace RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;

public interface IGraphReceiver
{
    Task<GraphDto> GetGraph();
}