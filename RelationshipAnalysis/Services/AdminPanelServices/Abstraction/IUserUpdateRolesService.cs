using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.AdminPanelServices.Abstraction;

public interface IUserUpdateRolesService
{
    Task<ActionResponse<MessageDto>> UpdateUserRolesAsync(User user, List<string> newRoles);
}