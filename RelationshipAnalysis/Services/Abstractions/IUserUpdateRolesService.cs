using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Models;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IUserUpdateRolesService
{
    Task<ActionResponse<MessageDto>> UpdateUserRolesAsync(User user, List<string> newRoles);
}