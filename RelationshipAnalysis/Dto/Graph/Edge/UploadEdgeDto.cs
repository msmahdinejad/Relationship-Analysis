using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RelationshipAnalysis.Dto.Graph.Edge;

public class UploadEdgeDto
{
    [Required] public string EdgeCategoryName { get; set; }

    [Required] public string UniqueKeyHeaderName { get; set; }

    [Required] public IFormFile File { get; set; }

    [Required] public string SourceNodeCategoryName { get; set; }

    [Required] public string TargetNodeCategoryName { get; set; }

    [Required] public string SourceNodeHeaderName { get; set; }

    [Required] public string TargetNodeHeaderName { get; set; }
}