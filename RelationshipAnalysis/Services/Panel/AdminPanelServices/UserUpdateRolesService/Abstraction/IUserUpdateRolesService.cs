using System.Collections.Generic;
using System.Threading.Tasks;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.UserUpdateRolesService.Abstraction;

public interface IUserUpdateRolesService
{
    Task<ActionResponse<MessageDto>> UpdateUserRolesAsync(User user, List<string> newRoles);
}