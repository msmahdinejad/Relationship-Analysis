using AutoMapper;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService;

public class UserUpdateInfoService(
    IUserUpdateInfoServiceValidator validator,
    IUserUpdater userUpdater,
    IMapper mapper) : IUserUpdateInfoService
{
    public async Task<ActionResponse<MessageDto>> UpdateUserAsync(User user, UserUpdateInfoDto userUpdateInfoDto,
        HttpResponse response)
    {
        var validateResult = await validator.Validate(user, userUpdateInfoDto);
        if (validateResult.StatusCode != StatusCodeType.Success)
            return validateResult;

        await UpdateInfo(user, userUpdateInfoDto);

        return validateResult;
    }

    private async Task UpdateInfo(User user, UserUpdateInfoDto userUpdateInfoDto)
    {
        mapper.Map(userUpdateInfoDto, user);
        await userUpdater.UpdateUserAsync(user);
    }
}