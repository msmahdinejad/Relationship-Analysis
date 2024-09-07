using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService.Abstraction;

public interface IAllUserDtoCreator
{
    Task<GetAllUsersDto> Create(List<User> users);
}