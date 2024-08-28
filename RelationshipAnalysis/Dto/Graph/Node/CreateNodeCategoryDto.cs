using System.ComponentModel.DataAnnotations;

namespace RelationshipAnalysis.Dto.Graph.Node;

public class CreateNodeCategoryDto
{
    [Required] public string NodeCategoryName { get; init; }
}