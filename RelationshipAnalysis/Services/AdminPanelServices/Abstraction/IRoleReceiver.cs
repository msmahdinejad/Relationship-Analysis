namespace RelationshipAnalysis.Services.AdminPanelServices.Abstraction;

public interface IRoleReceiver
{
    List<string> ReceiveRoles(int userId);

    List<string> ReceiveAllRoles();
}