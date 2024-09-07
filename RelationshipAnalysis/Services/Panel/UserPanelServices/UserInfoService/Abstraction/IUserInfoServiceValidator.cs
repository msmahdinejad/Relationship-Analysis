using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService.Abstraction;

public interface IUserInfoServiceValidator
{
    Task<ActionResponse<UserOutputInfoDto>> Validate(User user);
}