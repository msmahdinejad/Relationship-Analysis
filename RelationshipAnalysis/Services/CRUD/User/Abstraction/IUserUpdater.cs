namespace RelationshipAnalysis.Services.CRUD.User.Abstraction;

public interface IUserUpdater
{
    Task UpdateUserAsync(Models.Auth.User user);
}