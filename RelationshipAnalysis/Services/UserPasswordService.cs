using System.Security.Claims;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

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