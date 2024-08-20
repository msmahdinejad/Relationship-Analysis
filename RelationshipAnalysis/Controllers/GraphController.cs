using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Controllers;


[ApiController]
[Authorize]
[Route("api/[controller]/[action]")]
public class GraphController(IGraphReceiver graphReceiver, INodesAdditionService nodesAdditionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetGraph()
    {
        return Ok( await graphReceiver.GetGraph());
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