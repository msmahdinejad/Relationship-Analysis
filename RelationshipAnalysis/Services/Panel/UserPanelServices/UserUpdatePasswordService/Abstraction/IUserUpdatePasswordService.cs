using System.Threading.Tasks;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;
using UserPasswordInfoDto = RelationshipAnalysis.Dto.Panel.User.UserPasswordInfoDto;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdatePasswordService.Abstraction;

public interface IUserUpdatePasswordService
{
    Task<ActionResponse<MessageDto>> UpdatePasswordAsync(User user, UserPasswordInfoDto passwordInfoDto);
}