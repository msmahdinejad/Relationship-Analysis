using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CRUD.UserRole.Abstraction;

namespace RelationshipAnalysis.Services.CRUD.UserRole;

public class UserRolesRemover(IServiceProvider serviceProvider) : IUserRolesRemover
{
    public async Task RemoveUserRoles(Models.Auth.User user)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var allUserRoles = await context.UserRoles.ToListAsync();
        var userRoles = allUserRoles.FindAll(r => r.UserId == user.Id);
        context.RemoveRange(userRoles);
        await context.SaveChangesAsync();
    }
}