using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;

namespace RelationshipAnalysis.Services.CRUD.User;

public class UserAdder(IServiceProvider serviceProvider) : IUserAdder
{
    public async Task<Models.Auth.User> AddUserAsync(Models.Auth.User user)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }
}