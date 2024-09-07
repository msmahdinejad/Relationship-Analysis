using System.ComponentModel.DataAnnotations;

namespace RelationshipAnalysis.Dto.Graph;

public class ExpansionDto
{
    [Required] public int NodeId { get; set; }

    [Required] public int SourceCategoryId { get; set; }

    [Required] public int TargetCategoryId { get; set; }

    [Required] public int EdgeCategoryId { get; set; }
}