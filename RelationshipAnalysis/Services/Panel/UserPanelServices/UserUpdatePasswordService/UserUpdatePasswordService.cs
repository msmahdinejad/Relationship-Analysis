using System.Threading.Tasks;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.AuthServices.Abstraction;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdatePasswordService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdatePasswordService;

public class UserUpdatePasswordService(
    IUserUpdatePasswordServiceValidator validator,
    IUserUpdater userUpdater,
    IPasswordHasher passwordHasher) : IUserUpdatePasswordService
{
    public async Task<ActionResponse<MessageDto>> UpdatePasswordAsync(User user, UserPasswordInfoDto passwordInfoDto)
    {
        var validateResult = await validator.Validate(user, passwordInfoDto);
        if (validateResult.StatusCode != StatusCodeType.Success)
            return validateResult;
        
        await UpdatePassword(user, passwordInfoDto);

        return validateResult;
    }

    private async Task UpdatePassword(User user, UserPasswordInfoDto passwordInfoDto)
    {
        user.PasswordHash = passwordHasher.HashPassword(passwordInfoDto.NewPassword);
        await userUpdater.UpdateUserAsync(user);
    }
}