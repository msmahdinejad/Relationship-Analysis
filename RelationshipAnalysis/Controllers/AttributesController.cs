using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AttributesController(
    [FromKeyedServices("node")] IAttributesReceiver nodeAttributeReceiver,
    [FromKeyedServices("edge")] IAttributesReceiver edgeAttributeReceiver) : ControllerBase
{
    [HttpGet("nodes")]
    public async Task<IActionResult> GetNodeAttributes(string nodeCategoryName)
    {
        var result = await nodeAttributeReceiver.GetAllAttributes(nodeCategoryName);
        return Ok(result);
    }

    [HttpGet("edges")]
    public async Task<IActionResult> GetEdgeAttributes(string edgeCategoryName)
    {
        var result = await edgeAttributeReceiver.GetAllAttributes(edgeCategoryName);
        return Ok(result);
    }
}