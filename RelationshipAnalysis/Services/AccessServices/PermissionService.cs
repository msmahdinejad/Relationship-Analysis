using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Services.AccessServices.Abstraction;

namespace RelationshipAnalysis.Services.AccessServices;

public class PermissionService(ApplicationDbContext context) : IPermissionService
{
    public async Task<ActionResponse<PermissionDto>> GetPermissionsAsync(ClaimsPrincipal userClaims)
    {
        var unionList = await CreatPrmissionsList(userClaims);

        var permissions = JsonConvert.SerializeObject(unionList);

        var permissionDto = new PermissionDto(permissions);

        var result = new ActionResponse<PermissionDto>
        {
            Data = permissionDto
        };

        return result;
    }

    private async Task<List<string>?> CreatPrmissionsList(ClaimsPrincipal userClaims)
    {
        var unionList = new HashSet<string>();
        var roleNames = userClaims.FindAll(ClaimTypes.Role).Select(c => c.Value).Distinct();

        foreach (var roleName in roleNames)
        {
            var role = await context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);


            var newList = JsonConvert.DeserializeObject<List<string>>(role.Permissions) ?? [];
            unionList.UnionWith(newList);
        }

        return unionList.ToList();
    }
}