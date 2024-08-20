using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.AdminPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.AdminPanelServices;

public class RoleReceiver(IServiceProvider serviceProvider) : IRoleReceiver
{
    public async Task<List<string>> ReceiveRoles(int userId)
    {
        
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return (await context.UserRoles.ToListAsync()).FindAll(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name).ToList();
    }

    public async Task<List<string>> ReceiveAllRoles()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await context.Roles.Select(x => x.Name).ToListAsync();
    }
}