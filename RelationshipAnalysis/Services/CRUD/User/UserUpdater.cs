using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;

namespace RelationshipAnalysis.Services.CRUD.User;

public class UserUpdater(IServiceProvider serviceProvider) : IUserUpdater
{
    public async Task UpdateUserAsync(Models.Auth.User user)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Update(user);
        await context.SaveChangesAsync();
    }
}