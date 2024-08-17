using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Models;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IUserDeleteService
{
    Task<ActionResponse<MessageDto>> DeleteUser(User user);
}