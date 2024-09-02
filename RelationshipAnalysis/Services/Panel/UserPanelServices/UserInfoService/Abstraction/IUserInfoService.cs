using System.Threading.Tasks;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;
using UserOutputInfoDto = RelationshipAnalysis.Dto.Panel.User.UserOutputInfoDto;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService.Abstraction;

public interface IUserInfoService
{
    Task<ActionResponse<UserOutputInfoDto>> GetUser(User user);
}