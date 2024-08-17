using System.Security.Claims;
using RelationshipAnalysis.Models;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IUserReceiver
{
    Task<User> ReceiveUserAsync(ClaimsPrincipal userClaims);
    Task<User> ReceiveUserAsync(int id);
    List<User> ReceiveAllUser(int page, int size);
}