using System.ComponentModel.DataAnnotations;

namespace RelationshipAnalysis.Dto.Graph;

public class UploadNodeDto
{
    [Required]
    public string NodeCategoryName { get; set; }
    [Required]
    public string UniqueAttributeHeaderName { get; set; }
    [Required]
    public IFormFile File { get; set; }
}