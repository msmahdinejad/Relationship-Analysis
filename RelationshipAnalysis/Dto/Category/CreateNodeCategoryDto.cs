using System.ComponentModel.DataAnnotations;

namespace RelationshipAnalysis.Dto.Category;

public class CreateNodeCategoryDto
{
    [Required] public string NodeCategoryName { get; init; }
}