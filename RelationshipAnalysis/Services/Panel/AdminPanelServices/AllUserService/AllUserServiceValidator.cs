using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService;

public class AllUserServiceValidator : IAllUserServiceValidator
{
    public Task<ActionResponse<GetAllUsersDto>> Validate(List<User> users)
    {
        return Task.FromResult(users.IsNullOrEmpty() ? NotFoundResult() : SuccessResult());
    }

    private ActionResponse<GetAllUsersDto> SuccessResult()
    {
        return new ActionResponse<GetAllUsersDto>
        {
            StatusCode = StatusCodeType.Success
        };
    }

    private ActionResponse<GetAllUsersDto> NotFoundResult()
    {
        return new ActionResponse<GetAllUsersDto>
        {
            StatusCode = StatusCodeType.NotFound
        };
    }
}