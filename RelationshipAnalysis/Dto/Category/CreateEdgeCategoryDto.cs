using System.ComponentModel.DataAnnotations;

namespace RelationshipAnalysis.Dto.Category;

public class CreateEdgeCategoryDto
{
    [Required] public string EdgeCategoryName { get; init; }
}