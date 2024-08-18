using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.AdminPanelServices.Abstraction;

public interface IUserDeleteService
{
    Task<ActionResponse<MessageDto>> DeleteUser(User user);
}