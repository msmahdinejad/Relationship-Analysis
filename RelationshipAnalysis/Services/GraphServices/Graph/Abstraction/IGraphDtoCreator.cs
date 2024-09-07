using RelationshipAnalysis.Dto.Graph;

namespace RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;

public interface IGraphDtoCreator
{
    GraphDto CreateResultGraphDto(List<Models.Graph.Node.Node> contextNodes, List<Models.Graph.Edge.Edge> contextEdges);
}