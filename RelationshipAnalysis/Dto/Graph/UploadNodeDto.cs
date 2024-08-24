using System.ComponentModel.DataAnnotations;

namespace RelationshipAnalysis.Dto.Graph;

public class UploadNodeDto
{
    [Required]
    public string NodeCategoryName { get; set; }
    [Required]
    public string UniqueKeyHeaderName { get; set; }
    [Required]
    public IFormFile File { get; set; }
}