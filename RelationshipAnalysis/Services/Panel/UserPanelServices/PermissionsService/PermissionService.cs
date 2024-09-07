using Newtonsoft.Json;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.CRUD.Permissions.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.PermissionsService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.PermissionsService;

public class PermissionService(IPermissionsReceiver permissionsReceiver) : IPermissionService
{
    public async Task<ActionResponse<PermissionDto>> GetPermissionsAsync(User user)
    {
        var unionList = await permissionsReceiver.ReceivePermissionsAsync(user);

        var permissions = JsonConvert.SerializeObject(unionList);

        var permissionDto = new PermissionDto(permissions);

        var result = new ActionResponse<PermissionDto>
        {
            Data = permissionDto
        };

        return result;
    }
}