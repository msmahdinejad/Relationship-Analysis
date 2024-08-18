using RelationshipAnalysis.Dto;

namespace RelationshipAnalysis.Services.AdminPanelServices.Abstraction;

public interface IUserCreateService
{
    Task<ActionResponse<MessageDto>> CreateUser(CreateUserDto createUserDto);
}