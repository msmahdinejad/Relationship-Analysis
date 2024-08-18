using System.Security.Claims;
using RelationshipAnalysis.Dto;

namespace RelationshipAnalysis.Services.AccessServices.Abstraction;

public interface IPermissionService
{
    Task<ActionResponse<PermissionDto>> GetPermissionsAsync(ClaimsPrincipal userClaims);
}