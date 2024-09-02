using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RelationshipAnalysis.Dto.Graph.Node;

public class UploadNodeDto
{
    [Required] public string NodeCategoryName { get; set; }

    [Required] public string UniqueKeyHeaderName { get; set; }

    [Required] public IFormFile File { get; set; }
}