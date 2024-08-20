namespace RelationshipAnalysis.Services.AdminPanelServices.Abstraction;

public interface IRoleReceiver
{
    Task<List<string>> ReceiveRoles(int userId);

    Task<List<string>> ReceiveAllRoles();
}