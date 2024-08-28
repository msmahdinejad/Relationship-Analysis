using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AttributesController([FromKeyedServices("node")] IAttributesReceiver nodeAttributeReceiver,[FromKeyedServices("edge")] IAttributesReceiver edgeAttributeReceiver) : ControllerBase
{
    [HttpGet("nodes")]
    public async Task<IActionResult> GetNodeAttributes(int nodeCategoryId)
    {
        var result = await nodeAttributeReceiver.GetAllAttributes(nodeCategoryId);
        return Ok(result);
    }
    [HttpGet("edges")]
    public async Task<IActionResult> GetEdgeAttributes(int edgeCategoryId)
    {
        var result = await edgeAttributeReceiver.GetAllAttributes(edgeCategoryId);
        return Ok(result);
    }
}