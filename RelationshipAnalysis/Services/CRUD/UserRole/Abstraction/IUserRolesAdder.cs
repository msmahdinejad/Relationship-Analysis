namespace RelationshipAnalysis.Services.CRUD.UserRole.Abstraction;

public interface IUserRolesAdder
{
    Task AddUserRoles(List<Models.Auth.Role> roles, Models.Auth.User user);
}