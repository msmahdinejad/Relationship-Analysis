using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Dto.Category;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Services.CategoryServices.EdgeCategory.Abstraction;
using RelationshipAnalysis.Services.CategoryServices.NodeCategory.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EdgeController(
    ICreateEdgeCategoryService createEdgeCategoryService,
    IEdgeCategoryReceiver edgeCategoryReceiver,
    IEdgesAdditionService edgesAdditionService)
    : ControllerBase
{

    [HttpGet("categories")]
    public async Task<IActionResult> GetAllEdgeCategories()
    {
        var result = await edgeCategoryReceiver.GetAllEdgeCategories();
        return Ok(result);
    }
    
    [HttpPost("categories")]
    public async Task<IActionResult> CreateEdgeCategory([FromBody] CreateEdgeCategoryDto createEdgeCategoryDto)
    {
        var result = await createEdgeCategoryService.CreateEdgeCategory(createEdgeCategoryDto);
        return StatusCode((int)result.StatusCode, result.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> UploadNode([FromForm] UploadEdgeDto uploadEdgeDto)
    {
        if (uploadEdgeDto.File == null || uploadEdgeDto.File.Length == 0)
        {
            return BadRequest(Resources.NoFileUploadedMessage);
        }

        var result = await edgesAdditionService.AddEdges(uploadEdgeDto);
        return StatusCode((int)result.StatusCode, result.Data);
    }
}
