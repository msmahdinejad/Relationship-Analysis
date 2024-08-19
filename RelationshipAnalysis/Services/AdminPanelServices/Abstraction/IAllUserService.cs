using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.AdminPanelServices.Abstraction;

public interface IAllUserService
{
    ActionResponse<GetAllUsersDto> GetAllUser(List<User> users);
    int ReceiveAllUserCount();
}