using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.AdminPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.AdminPanelServices;

public class UserUpdateRolesService(ApplicationDbContext context) : IUserUpdateRolesService
{
    public async Task<ActionResponse<MessageDto>> UpdateUserRolesAsync(User user, List<string> newRoles)
    {
        if (newRoles.IsNullOrEmpty())
        {
            return BadRequestResult(Resources.EmptyRolesMessage);
        }

        var invalidRoles = newRoles.FindAll(r => !context.Roles.Select(R => R.Name)
            .Contains(r));
        if (invalidRoles.Any())
        {
            return BadRequestResult(Resources.InvalidRolesListMessage);
        }
        
        await RemoveUserRoles(user);
        await AddUserRoles(newRoles, user);
        return SuccessResult();
    }
    
    
    private async Task RemoveUserRoles(User user)
    {
        var userRoles = context.UserRoles.ToList().FindAll(r => r.UserId == user.Id);
        context.RemoveRange(userRoles);
        await context.SaveChangesAsync();
    }

    private ActionResponse<MessageDto> SuccessResult()
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.SuccessfulUpdateRolesMessage),
            StatusCode = StatusCodeType.Success
        };
    }

    private async Task AddUserRoles(List<string> newRoles, User user)
    {
        var roles = newRoles.Select(r => context.Roles
            .SingleOrDefaultAsync(R => R.Name == r));
        roles.ToList().ForEach(r => context.UserRoles.Add(new UserRole()
            { RoleId = r.Id, UserId = user.Id }));
        await context.SaveChangesAsync();
    }

    private ActionResponse<MessageDto> BadRequestResult(string message)
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(message),
            StatusCode = StatusCodeType.BadRequest
        };
    }
}