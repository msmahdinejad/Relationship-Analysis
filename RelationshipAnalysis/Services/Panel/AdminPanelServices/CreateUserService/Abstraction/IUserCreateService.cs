using RelationshipAnalysis.Dto;
using CreateUserDto = RelationshipAnalysis.Dto.Panel.Admin.CreateUserDto;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.CreateUserService.Abstraction;

public interface IUserCreateService
{
    Task<ActionResponse<MessageDto>> CreateUser(CreateUserDto createUserDto);
}