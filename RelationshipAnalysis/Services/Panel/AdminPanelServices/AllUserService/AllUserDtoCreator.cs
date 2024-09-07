using AutoMapper;
using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.CRUD.Role.Abstraction;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService;

public class AllUserDtoCreator(IMapper mapper, IRoleReceiver rolesReceiver, IUserReceiver userReceiver)
    : IAllUserDtoCreator
{
    public async Task<GetAllUsersDto> Create(List<User> users)
    {
        var userOutputs = new List<UserOutputInfoDto>();
        foreach (var user in users)
        {
            var data = new UserOutputInfoDto();
            mapper.Map(user, data);
            data.Roles = await rolesReceiver.ReceiveRoleNamesAsync(user.Id);

            userOutputs.Add(data);
        }

        return new GetAllUsersDto
        {
            Users = userOutputs,
            AllUserCount = await userReceiver.ReceiveAllUserCountAsync()
        };
    }
}