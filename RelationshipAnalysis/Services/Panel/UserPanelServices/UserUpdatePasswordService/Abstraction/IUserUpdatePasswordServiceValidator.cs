using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdatePasswordService.Abstraction;

public interface IUserUpdatePasswordServiceValidator
{
    Task<ActionResponse<MessageDto>> Validate(User user, UserPasswordInfoDto passwordInfoDto);
}