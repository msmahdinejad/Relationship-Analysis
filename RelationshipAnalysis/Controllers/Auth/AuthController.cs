using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Dto.Auth;
using RelationshipAnalysis.Services.AuthServices.Abstraction;

namespace RelationshipAnalysis.Controllers.Auth;

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