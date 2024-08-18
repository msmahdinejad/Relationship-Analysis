using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices.Abstraction;

namespace RelationshipAnalysis.Services.UserPanelServices;

public class UserPasswordService(ApplicationDbContext context, IPasswordVerifier passwordVerifier, IPasswordHasher passwordHasher) : IUserPasswordService
{
    public async Task<ActionResponse<MessageDto>> UpdatePasswordAsync(User user, UserPasswordInfoDto passwordInfoDto)
    {
        if (user is null)
        {
            return NotFoundResult();
        }
        if (!passwordVerifier.VerifyPasswordHash(passwordInfoDto.OldPassword, user.PasswordHash))
        {
            return WrongPasswordResult();
        }
        user.PasswordHash = passwordHasher.HashPassword(passwordInfoDto.NewPassword);
        context.Update(user);
        await context.SaveChangesAsync();

        return SuccessResult();
    }
    
    private ActionResponse<MessageDto> NotFoundResult()
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.UserNotFoundMessage),
            StatusCode = StatusCodeType.NotFound
        };
    }

    private ActionResponse<MessageDto> WrongPasswordResult()
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.WrongOldPasswordMessage),
            StatusCode = StatusCodeType.BadRequest
        };
    }
    
    private ActionResponse<MessageDto> SuccessResult()
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.SuccessfulUpdateUserMessage),
            StatusCode = StatusCodeType.Success
        };
    }


}