using System.Collections.Generic;
using System.Threading.Tasks;

namespace RelationshipAnalysis.Services.CRUD.Permissions.Abstraction;

public interface IPermissionsReceiver
{
    Task<List<string>> ReceivePermissionsAsync(Models.Auth.User user);
}