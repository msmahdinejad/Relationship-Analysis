using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService;

public class UserInfoServiceValidator : IUserInfoServiceValidator
{
    public async Task<ActionResponse<UserOutputInfoDto>> Validate(User user)
    {
        if (user is null) return NotFoundResult();
        return await SuccessResult();
    }

    private Task<ActionResponse<UserOutputInfoDto>> SuccessResult()
    {
        return Task.FromResult(new ActionResponse<UserOutputInfoDto>
        {
            StatusCode = StatusCodeType.Success
        });
    }

    private ActionResponse<UserOutputInfoDto> NotFoundResult()
    {
        return new ActionResponse<UserOutputInfoDto>
        {
            StatusCode = StatusCodeType.NotFound
        };
    }
}