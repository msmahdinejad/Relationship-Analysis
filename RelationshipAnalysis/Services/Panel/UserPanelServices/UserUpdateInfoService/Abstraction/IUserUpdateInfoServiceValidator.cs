using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService.Abstraction;

public interface IUserUpdateInfoServiceValidator
{
    Task<ActionResponse<MessageDto>> Validate(User user, UserUpdateInfoDto userUpdateInfoDto);
}