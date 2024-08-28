using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Dto.Graph.Node;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Graph;

public class GraphDtoCreator : IGraphDtoCreator
{
    
    public GraphDto CreateResultGraphDto(List<Models.Graph.Node.Node> contextNodes, List<Models.Graph.Edge.Edge> contextEdges)
    {
        if (contextEdges == null || contextNodes == null) throw new ArgumentNullException();
        var resultGraphDto = new GraphDto();
        contextNodes.ForEach(n => resultGraphDto.Nodes.Add(new NodeDto
        {
            id = n.NodeId.ToString(),
            label = $"{n.NodeCategory.NodeCategoryName}/{n.NodeUniqueString}"
        }));
        contextEdges.ForEach(e => resultGraphDto.Edges.Add(new EdgeDto
        {
            id = e.EdgeId.ToString(),
            source = e.EdgeSourceNodeId.ToString(),
            target = e.EdgeDestinationNodeId.ToString()
        }));
        return resultGraphDto;
    }
}