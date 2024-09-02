using System.Threading.Tasks;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;
using PermissionDto = RelationshipAnalysis.Dto.Panel.User.PermissionDto;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.PermissionsService.Abstraction;

public interface IPermissionService
{
    Task<ActionResponse<PermissionDto>> GetPermissionsAsync(User user);
}