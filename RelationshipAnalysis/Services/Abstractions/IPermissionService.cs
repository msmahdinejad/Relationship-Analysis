using System.Security.Claims;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.DTO;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IPermissionService
{
    Task<ActionResponse<PermissionDto>> GetPermissionsAsync(ClaimsPrincipal userClaims);
}