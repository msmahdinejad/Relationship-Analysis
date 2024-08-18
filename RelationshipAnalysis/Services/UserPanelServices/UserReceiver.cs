using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.UserPanelServices;

public class UserReceiver(ApplicationDbContext context) : IUserReceiver
{
    public async Task<User> ReceiveUserAsync(ClaimsPrincipal userClaims)
    {
        var currentId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var user = await context.Users.SingleOrDefaultAsync(u => u.Id == currentId);
        return user;
    }

    public async Task<User> ReceiveUserAsync(int id)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.Id == id);
        return user;
    }

    public List<User> ReceiveAllUser(int page, int size)
    {
        var users = context.Users.Skip(page * size).Take(size).ToList();
        return users;
    }
}