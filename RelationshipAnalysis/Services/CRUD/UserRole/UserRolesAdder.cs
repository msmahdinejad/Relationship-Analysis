using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CRUD.UserRole.Abstraction;

namespace RelationshipAnalysis.Services.CRUD.UserRole;

public class UserRolesAdder(IServiceProvider serviceProvider) : IUserRolesAdder
{
    public async Task AddUserRoles(List<Models.Auth.Role> roles, Models.Auth.User user)
    {
        var userRoles = roles.Select(role => new Models.Auth.UserRole
        {
            RoleId = role.Id,
            UserId = user.Id
        }).ToList();

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.UserRoles.AddRangeAsync(userRoles);
        await context.SaveChangesAsync();
    }
}