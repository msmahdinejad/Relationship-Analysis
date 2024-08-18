using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.UserPanelServices.Abstraction;

public interface IUserInfoService
{
    ActionResponse<UserOutputInfoDto> GetUser(User user);
}