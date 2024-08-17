using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class RoleReceiver(ApplicationDbContext context) : IRoleReceiver
{
    public List<string> ReceiveRoles(int userId)
    {
        return context.UserRoles.ToList().FindAll(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name).ToList();
    }

    public List<string> ReceiveAllRoles()
    {
        return context.Roles.Select(x => x.Name).ToList();
    }
}