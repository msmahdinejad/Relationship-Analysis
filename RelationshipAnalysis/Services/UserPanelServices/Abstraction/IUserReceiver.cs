using System.Security.Claims;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.UserPanelServices.Abstraction;

public interface IUserReceiver
{
    Task<User> ReceiveUserAsync(ClaimsPrincipal userClaims);
    Task<User> ReceiveUserAsync(int id);
    List<User> ReceiveAllUser(int page, int size);
    
}