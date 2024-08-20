using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Controllers;


[ApiController]
[Authorize]
[Route("api/[controller]/[action]")]
public class GraphController(IGraphReceiver graphReceiver) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetGraph()
    {
        return Ok(await graphReceiver.GetGraph());
    }
}