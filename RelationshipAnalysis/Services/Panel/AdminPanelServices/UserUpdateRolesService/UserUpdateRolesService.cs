using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.CRUD.Role.Abstraction;
using RelationshipAnalysis.Services.CRUD.UserRole.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.UserUpdateRolesService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.UserUpdateRolesService;

public class UserUpdateRolesService(
    IUserUpdateRolesServiceValidator validator,
    IRoleReceiver roleReceiver,
    IUserRolesAdder userRolesAdder,
    IUserRolesRemover userRolesRemover) : IUserUpdateRolesService
{
    public async Task<ActionResponse<MessageDto>> UpdateUserRolesAsync(User user, List<string> newRoles)
    {
        var validateResult = await validator.Validate(user, newRoles);
        if (validateResult.StatusCode != StatusCodeType.Success)
            return validateResult;

        await UpdateUserRoles(user, newRoles);

        return validateResult;
    }

    private async Task UpdateUserRoles(User user, List<string> newRoles)
    {
        await userRolesRemover.RemoveUserRoles(user);
        var roles = await roleReceiver.ReceiveRolesListAsync(newRoles);
        await userRolesAdder.AddUserRoles(roles, user);
    }
}