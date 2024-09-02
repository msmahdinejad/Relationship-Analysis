using System.Collections.Generic;
using System.Threading.Tasks;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService.Abstraction;

public interface IAllUserServiceValidator
{
    Task<ActionResponse<GetAllUsersDto>> Validate(List<User> users);
}