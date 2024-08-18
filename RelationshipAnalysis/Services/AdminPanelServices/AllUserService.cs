using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.AdminPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.AdminPanelServices;

public class AllUserService(IMapper mapper, IRoleReceiver rolesReceiver) : IAllUserService
{
    public ActionResponse<List<UserOutputInfoDto>> GetAllUser(List<User> users)
    {
        if (users.IsNullOrEmpty())
        {
            return NotFoundResult();
        }

        var userOutputs = GetAllUserOutputs(users);

        return SuccessResult(userOutputs);
    }

    private List<UserOutputInfoDto> GetAllUserOutputs(List<User> users)
    {
        var userOutputs = new List<UserOutputInfoDto>();
        foreach (var user in users)
        {
            var data = new UserOutputInfoDto();
            mapper.Map(user, data);
            data.Roles = rolesReceiver.ReceiveRoles(user.Id);
            
            userOutputs.Add(data);
        }

        return userOutputs;
    }

    private ActionResponse<List<UserOutputInfoDto>> SuccessResult(List<UserOutputInfoDto> users)
    {
        return new ActionResponse<List<UserOutputInfoDto>>()
        {
            Data = users,
            StatusCode = StatusCodeType.Success
        };
    }
    
    private ActionResponse<List<UserOutputInfoDto>> NotFoundResult()
    {
        return new ActionResponse<List<UserOutputInfoDto>>()
        {
            StatusCode = StatusCodeType.NotFound
        };
    }
}