using System.Collections.Generic;
using System.Threading.Tasks;

namespace RelationshipAnalysis.Services.CRUD.Role.Abstraction;

public interface IRoleReceiver
{
    Task<List<string>> ReceiveRoleNamesAsync(int userId);

    Task<List<string>> ReceiveAllRolesAsync();

    Task<List<Models.Auth.Role>> ReceiveRolesListAsync(List<string> roleNames);
}