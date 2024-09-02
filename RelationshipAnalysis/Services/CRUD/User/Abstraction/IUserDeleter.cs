using System.Threading.Tasks;

namespace RelationshipAnalysis.Services.CRUD.User.Abstraction;

public interface IUserDeleter
{
    Task DeleteUserAsync(Models.Auth.User user);
}