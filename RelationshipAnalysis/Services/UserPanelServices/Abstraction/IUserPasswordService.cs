using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.UserPanelServices.Abstraction;

public interface IUserPasswordService
{
    Task<ActionResponse<MessageDto>> UpdatePasswordAsync(User user,
        UserPasswordInfoDto passwordInfoDto);
}