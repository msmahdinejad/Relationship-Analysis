namespace RelationshipAnalysis.Services.CRUD.UserRole.Abstraction;

public interface IUserRolesRemover
{
    Task RemoveUserRoles(Models.Auth.User user);
}