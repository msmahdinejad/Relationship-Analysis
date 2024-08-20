using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.AdminPanelServices.Abstraction;

public interface IAllUserService
{
    Task<ActionResponse<GetAllUsersDto>> GetAllUser(List<User> users);
    Task<int> ReceiveAllUserCount();
}