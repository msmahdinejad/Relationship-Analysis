using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AccessController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public AccessController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetPermissions()
    {
        var response = await _permissionService.GetPermissionsAsync(User);

        return StatusCode((int)response.StatusCode, response.Data);
    }
}