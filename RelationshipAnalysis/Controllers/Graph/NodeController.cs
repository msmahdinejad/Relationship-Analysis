using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Dto.Graph.Node;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

namespace RelationshipAnalysis.Controllers.Graph;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NodeController(
    ICreateNodeCategoryService createNodeCategoryService,
    INodeCategoryReceiver nodeCategoryReceiver,
    INodesAdditionService nodesAdditionService,
    [FromKeyedServices("node")] IInfoReceiver infoReceiver)
    : ControllerBase
{
    [HttpGet("categories")]
    public async Task<IActionResult> GetAllNodeCategories()
    {
        var result = await nodeCategoryReceiver.GetAllNodeCategories();
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetInfo(int nodeId)
    {
        var result = await infoReceiver.GetInfo(nodeId);
        return StatusCode((int)result.StatusCode, result.Data);
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
        if (uploadNodeDto.File == null || uploadNodeDto.File.Length == 0)
        {
            return BadRequest(Resources.NoFileUploadedMessage);
        }

        var result = await nodesAdditionService.AddNodes(uploadNodeDto);
        return StatusCode((int)result.StatusCode, result.Data);
    }
}
