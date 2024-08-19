using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.AdminPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.AdminPanelServices;

public class RoleReceiver(IServiceProvider serviceProvider) : IRoleReceiver
{
    public List<string> ReceiveRoles(int userId)
    {
        
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return context.UserRoles.ToList().FindAll(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name).ToList();
    }

    public List<string> ReceiveAllRoles()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return context.Roles.Select(x => x.Name).ToList();
    }
}