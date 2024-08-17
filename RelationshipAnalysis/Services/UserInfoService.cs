
using AutoMapper;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class UserInfoService(IRoleReceiver rolesReceiver, IMapper mapper) : IUserInfoService
{
    public ActionResponse<UserOutputInfoDto> GetUser(User user)
    {
        if (user is null)
        {
            return NotFoundResult();
        }
        return SuccessResult(user);
    }

    private ActionResponse<UserOutputInfoDto> SuccessResult(User user)
    {
        var result = new UserOutputInfoDto();
        mapper.Map(user, result);
        result.Roles = rolesReceiver.ReceiveRoles(user.Id);
        
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