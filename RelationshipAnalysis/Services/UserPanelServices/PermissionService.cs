using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.UserPanelServices;

public class PermissionService(IServiceProvider serviceProvider) : IPermissionService
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
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var role = await context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);


            var newList = JsonConvert.DeserializeObject<List<string>>(role.Permissions) ?? [];
            unionList.UnionWith(newList);
        }

        return unionList.ToList();
    }
}