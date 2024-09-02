using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.CRUD.Role.Abstraction;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.CreateUserService.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.UserDeleteService.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.UserUpdateRolesService.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdatePasswordService.Abstraction;

namespace RelationshipAnalysis.Controllers.Panel;

[Authorize(Roles = nameof(RoleTypes.Admin))]
[ApiController]
[Route("api/[controller]")]
public class AdminController(
    IUserUpdateRolesService userUpdateRolesService,
    IUserCreateService userCreateService,
    IRoleReceiver roleReceiver,
    IAllUserService allUserService,
    IUserInfoService userInfoService,
    IUserDeleteService userDeleteService,
    IUserUpdateInfoService userUpdateInfoService,
    IUserUpdatePasswordService updatePasswordService,
    IUserReceiver userReceiver) : ControllerBase
{
    [HttpGet("users/{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await userReceiver.ReceiveUserAsync(id);
        var result = await userInfoService.GetUser(user);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUser([FromQuery] int page, [FromQuery] int size)
    {
        var users = userReceiver.ReceiveAllUserAsync(page, size);
        var result = await allUserService.GetAllUser(users);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await roleReceiver.ReceiveAllRolesAsync();
        return Ok(roles);
    }

    [HttpPut("users/{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateInfoDto userUpdateInfoDto)
    {
        var user = await userReceiver.ReceiveUserAsync(id);
        var result = await userUpdateInfoService.UpdateUserAsync(user, userUpdateInfoDto, Response);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpPatch("users/{id:int}/password")]
    public async Task<IActionResult> UpdatePassword(int id, UserPasswordInfoDto passwordInfoDto)
    {
        var user = await userReceiver.ReceiveUserAsync(id);
        var result = await updatePasswordService.UpdatePasswordAsync(user, passwordInfoDto);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpDelete("users/{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var currentUser = await userReceiver.ReceiveUserAsync(User);
        var user = await userReceiver.ReceiveUserAsync(id);
        if (id == currentUser.Id)
            return StatusCode((int)StatusCodeType.BadRequest, Resources.DeleteAccountAccessErrorMessage);

        var result = await userDeleteService.DeleteUser(user);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
    {
        var result = await userCreateService.CreateUser(createUserDto);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpPatch("users/{id:int}/roles")]
    public async Task<IActionResult> UpdateRoles(int id, List<string> newRoles)
    {
        var user = await userReceiver.ReceiveUserAsync(id);
        var result = await userUpdateRolesService.UpdateUserRolesAsync(user, newRoles);
        return StatusCode((int)result.StatusCode, result.Data);
    }
}