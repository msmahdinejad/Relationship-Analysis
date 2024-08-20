using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services.AdminPanelServices.Abstraction;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction;

namespace RelationshipAnalysis.Controllers;

[Authorize(Roles = nameof(RoleTypes.Admin))]
[ApiController]
[Route("api/[controller]/[action]")]
public class AdminController(
    IUserUpdateRolesService userUpdateRolesService,
    IUserCreateService userCreateService,
    IRoleReceiver roleReceiver,
    IAllUserService allUserService,
    IUserInfoService userInfoService,
    IUserDeleteService userDeleteService,
    IUserUpdateInfoService userUpdateInfoService,
    IUserPasswordService passwordService,
    IUserReceiver userReceiver) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await userReceiver.ReceiveUserAsync(id);
        var result = await userInfoService.GetUser(user);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUser([FromQuery] int page, [FromQuery] int size)
    {
        var users = userReceiver.ReceiveAllUser(page, size);
        var result = await allUserService.GetAllUser(users);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpGet]
    public IActionResult GetAllRoles()
    {
        var roles = roleReceiver.ReceiveAllRoles();
        return Ok(roles);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateInfoDto userUpdateInfoDto)
    {
        var user = await userReceiver.ReceiveUserAsync(id);
        var result = await userUpdateInfoService.UpdateUserAsync(user, userUpdateInfoDto, Response);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePassword(int id, UserPasswordInfoDto passwordInfoDto)
    {
        var user = await userReceiver.ReceiveUserAsync(id);
        var result = await passwordService.UpdatePasswordAsync(user, passwordInfoDto);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var currentUser = await userReceiver.ReceiveUserAsync(User);
        var user = await userReceiver.ReceiveUserAsync(id);
        if (id == currentUser.Id)
        {
            return StatusCode((int)StatusCodeType.BadRequest, Resources.DeleteAccountAccessErrorMessage);
        }

        var result = await userDeleteService.DeleteUser(user);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
    {
        var result = await userCreateService.CreateUser(createUserDto);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateRoles(int id, List<string> newRoles)
    {
        var user = await userReceiver.ReceiveUserAsync(id);
        var result = await userUpdateRolesService.UpdateUserRolesAsync(user, newRoles);
        return StatusCode((int)result.StatusCode, result.Data);
    }
}