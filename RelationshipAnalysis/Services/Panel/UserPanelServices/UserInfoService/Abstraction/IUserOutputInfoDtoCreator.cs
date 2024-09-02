using System.Threading.Tasks;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService.Abstraction;

public interface IUserOutputInfoDtoCreator
{
    Task<UserOutputInfoDto> Create(User user);
}