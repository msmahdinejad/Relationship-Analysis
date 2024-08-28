using System.ComponentModel.DataAnnotations;

namespace RelationshipAnalysis.Dto.Graph.Edge;

public class CreateEdgeCategoryDto
{
    [Required] public string EdgeCategoryName { get; init; }
}