using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CRUD.Role.Abstraction;

namespace RelationshipAnalysis.Services.CRUD.Role;

public class RoleReceiver(IServiceProvider serviceProvider) : IRoleReceiver
{
    public async Task<List<string>> ReceiveRoleNamesAsync(int userId)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return (await context.UserRoles.ToListAsync()).FindAll(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name).ToList();
    }

    public async Task<List<string>> ReceiveAllRolesAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await context.Roles.Select(x => x.Name).ToListAsync();
    }

    public async Task<List<Models.Auth.Role>> ReceiveRolesListAsync(List<string> roleNames)
    {
        var roles = new List<Models.Auth.Role>();

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        foreach (var roleName in roleNames)
        {
            var role = await context.Roles
                .SingleOrDefaultAsync(r => r.Name == roleName);

            if (role != null) roles.Add(role);
        }

        return roles;
    }
}