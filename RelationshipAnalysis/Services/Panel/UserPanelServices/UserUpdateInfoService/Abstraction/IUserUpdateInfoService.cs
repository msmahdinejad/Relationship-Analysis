using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models.Auth;
using UserUpdateInfoDto = RelationshipAnalysis.Dto.Panel.User.UserUpdateInfoDto;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService.Abstraction;

public interface IUserUpdateInfoService
{
    Task<ActionResponse<MessageDto>> UpdateUserAsync(User user, UserUpdateInfoDto userUpdateInfoDto,
        HttpResponse response);
}