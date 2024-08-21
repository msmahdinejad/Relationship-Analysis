using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices.Abstraction;

namespace RelationshipAnalysis.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly ILoginService _loginService;

    public AuthController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDto loginModel)
    {
        var response = await _loginService.LoginAsync(loginModel, Response);

        return StatusCode((int)response.StatusCode, response.Data);
    }
}