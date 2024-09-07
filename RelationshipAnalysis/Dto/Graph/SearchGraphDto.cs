using System.ComponentModel.DataAnnotations;

namespace RelationshipAnalysis.Dto.Graph;

public class SearchGraphDto
{
    [Required] public string SourceCategoryName { get; set; }

    [Required] public string TargetCategoryName { get; set; }

    [Required] public string EdgeCategoryName { get; set; }

    [Required] public Dictionary<string, string> SourceCategoryClauses { get; set; }

    [Required] public Dictionary<string, string> TargetCategoryClauses { get; set; }

    [Required] public Dictionary<string, string> EdgeCategoryClauses { get; set; }
}