using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Dto.Graph.Node;
using RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

namespace RelationshipAnalysis.Controllers.Graph;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NodeController(
    ICreateNodeCategoryService createNodeCategoryService,
    INodeCategoryReceiver nodeCategoryReceiver,
    INodesAdditionService nodesAdditionService)
    : ControllerBase
{
    [HttpGet("categories")]
    public async Task<IActionResult> GetAllNodeCategories()
    {
        var result = await nodeCategoryReceiver.GetAllNodeCategories();
        return Ok(result);
    }


    [HttpPost("categories")]
    public async Task<IActionResult> CreateNodeCategory([FromBody] CreateNodeCategoryDto createNodeCategoryDto)
    {
        var result = await createNodeCategoryService.CreateNodeCategory(createNodeCategoryDto);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> UploadNode([FromForm] UploadNodeDto uploadNodeDto)
    {
        if (uploadNodeDto.File.Length == 0) return BadRequest(Resources.NoFileUploadedMessage);

        var result = await nodesAdditionService.AddNodes(uploadNodeDto);
        return StatusCode((int)result.StatusCode, result.Data);
    }
}