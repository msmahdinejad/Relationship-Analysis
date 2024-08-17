using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IAllUserService
{
    ActionResponse<List<UserOutputInfoDto>> GetAllUser(List<User> users);
}