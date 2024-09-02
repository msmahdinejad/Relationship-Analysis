using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RelationshipAnalysis.Services.CRUD.User.Abstraction;

public interface IUserReceiver
{
    Task<int> ReceiveAllUserCountAsync();
    Task<Models.Auth.User> ReceiveUserAsync(ClaimsPrincipal userClaims);
    Task<Models.Auth.User> ReceiveUserAsync(int id);
    Task<Models.Auth.User> ReceiveUserAsync(string username);
    List<Models.Auth.User> ReceiveAllUserAsync(int page, int size);
}