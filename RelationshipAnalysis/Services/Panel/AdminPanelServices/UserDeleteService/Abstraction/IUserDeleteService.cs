using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.UserDeleteService.Abstraction;

public interface IUserDeleteService
{
    Task<ActionResponse<MessageDto>> DeleteUser(User user);
}