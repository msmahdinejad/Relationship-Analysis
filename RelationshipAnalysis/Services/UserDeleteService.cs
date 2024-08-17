using RelationshipAnalysis.Context;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class UserDeleteService(ApplicationDbContext context) : IUserDeleteService
{
    public async Task<ActionResponse<MessageDto>> DeleteUser(User user)
    {
        if (user is null)
        {
            return NotFoundResult();
        }

        context.Remove(user);
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

    private ActionResponse<MessageDto> SuccessResult()
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.SuccessfulDeleteUserMessage),
            StatusCode = StatusCodeType.Success
        };
    }
}