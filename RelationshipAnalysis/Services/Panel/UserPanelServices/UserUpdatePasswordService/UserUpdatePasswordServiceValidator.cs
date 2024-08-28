using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.AuthServices.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdatePasswordService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdatePasswordService;

public class UserUpdatePasswordServiceValidator(
    IPasswordVerifier passwordVerifier,
    IMessageResponseCreator messageResponseCreator) : IUserUpdatePasswordServiceValidator
{
    public Task<ActionResponse<MessageDto>> Validate(User user, UserPasswordInfoDto passwordInfoDto)
    {
        if (user is null) return Task.FromResult(messageResponseCreator.Create(StatusCodeType.NotFound, Resources.UserNotFoundMessage));
        if (!passwordVerifier.VerifyPasswordHash(passwordInfoDto.OldPassword, user.PasswordHash))
            return Task.FromResult(messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.WrongOldPasswordMessage));
        return Task.FromResult(messageResponseCreator.Create(StatusCodeType.Success, Resources.SuccessfulUpdateUserMessage));
    }
}