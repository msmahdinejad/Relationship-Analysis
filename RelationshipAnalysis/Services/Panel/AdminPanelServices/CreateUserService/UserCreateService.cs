using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.CRUD.Role.Abstraction;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;
using RelationshipAnalysis.Services.CRUD.UserRole.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.CreateUserService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.CreateUserService;

public class UserCreateService(
    ICreateUserDtoMapper mapper,
    IUserAdder userAdder,
    IRoleReceiver roleReceiver,
    IUserRolesAdder userRolesAdder,
    IUserCreateServiceValidator validator) : IUserCreateService
{
    public async Task<ActionResponse<MessageDto>> CreateUser(CreateUserDto createUserDto)
    {
        var validateResult = await validator.Validate(createUserDto);
        if (validateResult.StatusCode != StatusCodeType.Success) return validateResult;

        await AddUser(createUserDto);

        return validateResult;
    }

    private async Task AddUser(CreateUserDto createUserDto)
    {
        var user = mapper.Map(createUserDto);
        await userAdder.AddUserAsync(user);
        var roles = await roleReceiver.ReceiveRolesListAsync(createUserDto.Roles);
        await userRolesAdder.AddUserRoles(roles , user);
    }
}