using System.Threading.Tasks;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService;

public class UserInfoService(
    IUserOutputInfoDtoCreator userOutputInfoDtoCreator,
    IUserInfoServiceValidator validator) : IUserInfoService
{
    public async Task<ActionResponse<UserOutputInfoDto>> GetUser(User user)
    {
        var validateResult = await validator.Validate(user);
        if (validateResult.StatusCode != StatusCodeType.Success)
            return validateResult;

        validateResult.Data = await userOutputInfoDtoCreator.Create(user);
        
        return validateResult;
    }
}