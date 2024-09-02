using System.Collections.Generic;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Dto.Graph.Node;

namespace RelationshipAnalysis.Dto.Graph;

public class GraphDto
{
    public List<NodeDto> Nodes { get; set; } = [];
    public List<EdgeDto> Edges { get; set; } = [];
    public string Message { get; set; }
}