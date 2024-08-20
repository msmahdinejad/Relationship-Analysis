using AutoMapper;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.AdminPanelServices.Abstraction;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.UserPanelServices;

public class UserInfoService(IRoleReceiver rolesReceiver, IMapper mapper) : IUserInfoService
{
    public async Task<ActionResponse<UserOutputInfoDto>> GetUser(User user)
    {
        if (user is null)
        {
            return NotFoundResult();
        }
        return await SuccessResult(user);
    }

    private async Task<ActionResponse<UserOutputInfoDto>> SuccessResult(User user)
    {
        var result = new UserOutputInfoDto();
        mapper.Map(user, result);
        result.Roles = await rolesReceiver.ReceiveRoles(user.Id);
        
        return new ActionResponse<UserOutputInfoDto>()
        {
            Data = result,
            StatusCode = StatusCodeType.Success
        };
    }
    
    private ActionResponse<UserOutputInfoDto> NotFoundResult()
    {
        return new ActionResponse<UserOutputInfoDto>()
        {
            StatusCode = StatusCodeType.NotFound
        };
    }
}