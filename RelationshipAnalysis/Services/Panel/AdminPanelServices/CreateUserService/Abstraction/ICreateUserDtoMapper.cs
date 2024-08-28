using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.CreateUserService.Abstraction;

public interface ICreateUserDtoMapper
{
    User Map(CreateUserDto createUserDto);
}