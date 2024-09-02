using System.Threading.Tasks;

namespace RelationshipAnalysis.Services.CRUD.User.Abstraction;

public interface IUserAdder
{
    Task<Models.Auth.User> AddUserAsync(Models.Auth.User user);
}