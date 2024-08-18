using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.AdminPanelServices.Abstraction;

public interface IAllUserService
{
    ActionResponse<List<UserOutputInfoDto>> GetAllUser(List<User> users);
}