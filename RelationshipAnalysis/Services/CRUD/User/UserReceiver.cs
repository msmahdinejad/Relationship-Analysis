using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;

namespace RelationshipAnalysis.Services.CRUD.User;

public class UserReceiver(IServiceProvider serviceProvider) : IUserReceiver
{
    public async Task<int> ReceiveAllUserCountAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var users = await context.Users.ToListAsync();
        return users.Count;
    }

    public async Task<Models.Auth.User> ReceiveUserAsync(ClaimsPrincipal userClaims)
    {
        var currentId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = await context.Users.SingleOrDefaultAsync(u => u.Id == currentId);
        return user;
    }

    public async Task<Models.Auth.User> ReceiveUserAsync(int id)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = await context.Users.SingleOrDefaultAsync(u => u.Id == id);
        return user;
    }

    public async Task<Models.Auth.User> ReceiveUserAsync(string username)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .SingleOrDefaultAsync(u => u.Username == username);


        return user;
    }

    public List<Models.Auth.User> ReceiveAllUserAsync(int page, int size)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var users = context.Users.Skip(page * size).Take(size).ToList();
        return users;
    }
}