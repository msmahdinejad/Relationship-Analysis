using RelationshipAnalysis.Models;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IRoleReceiver
{
    List<string> ReceiveRoles(int userId);

    List<string> ReceiveAllRoles();
}