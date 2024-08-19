using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.AdminPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.AdminPanelServices;

public class UserDeleteService(IServiceProvider serviceProvider) : IUserDeleteService
{
    public async Task<ActionResponse<MessageDto>> DeleteUser(User user)
    {
        if (user is null)
        {
            return NotFoundResult();
        }

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
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