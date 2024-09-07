using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

namespace RelationshipAnalysis.Controllers.Graph;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EdgeController(
    ICreateEdgeCategoryService createEdgeCategoryService,
    IEdgeCategoryReceiver edgeCategoryReceiver,
    IEdgesAdditionService edgesAdditionService,
    [FromKeyedServices("edge")] IInfoReceiver infoReceiver)
    : ControllerBase
{
    [HttpGet("categories")]
    public async Task<IActionResult> GetAllEdgeCategories()
    {
        var result = await edgeCategoryReceiver.GetAllEdgeCategories();
        return Ok(result);
    }


    [HttpGet]
    public async Task<IActionResult> GetInfo(int edgeId)
    {
        var result = await infoReceiver.GetInfo(edgeId);
        return StatusCode((int)result.StatusCode, result.Data);
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
            return BadRequest(Resources.NoFileUploadedMessage);

        var result = await edgesAdditionService.AddEdges(uploadEdgeDto);
        return StatusCode((int)result.StatusCode, result.Data);
    }
}