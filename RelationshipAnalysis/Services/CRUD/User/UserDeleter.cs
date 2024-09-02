using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;

namespace RelationshipAnalysis.Services.CRUD.User;

public class UserDeleter(IServiceProvider serviceProvider) : IUserDeleter
{
    public async Task DeleteUserAsync(Models.Auth.User user)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Remove(user);
        await context.SaveChangesAsync();
    }
}