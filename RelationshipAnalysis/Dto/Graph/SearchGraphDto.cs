namespace RelationshipAnalysis.Dto.Graph;
using System.ComponentModel.DataAnnotations;


public class SearchGraphDto
{
    [Required]
    public string SourceCategoryName { get; set; }
    [Required]
    public string TargetCategoryName { get; set; }
    [Required]
    public string EdgeCategoryName { get; set; }
    [Required]
    public Dictionary<string, string> SourceCategoryClauses { get; set; }
    [Required]
    public Dictionary<string, string> TargetCategoryClauses { get; set; }
    [Required]
    public Dictionary<string, string> EdgeCategoryClauses { get; set; }
}