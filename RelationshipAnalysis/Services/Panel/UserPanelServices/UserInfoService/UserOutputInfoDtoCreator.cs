using System.Threading.Tasks;
using AutoMapper;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.CRUD.Role.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService;

public class UserOutputInfoDtoCreator(IMapper mapper, IRoleReceiver roleReceiver) : IUserOutputInfoDtoCreator
{
    public async Task<UserOutputInfoDto> Create(User user)
    {
        var result = new UserOutputInfoDto();
        mapper.Map(user, result);
        result.Roles = await roleReceiver.ReceiveRoleNamesAsync(user.Id);

        return result;
    }
}