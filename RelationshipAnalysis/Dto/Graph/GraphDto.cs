namespace RelationshipAnalysis.Dto.Graph;

public class GraphDto
{
    public List<NodeDto> nodes { get; set; } = new List<NodeDto>();
    public List<EdgeDto> edges { get; set; } = new List<EdgeDto>();
}