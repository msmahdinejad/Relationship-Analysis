using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.DTO;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IUserCreateService
{
    Task<ActionResponse<MessageDto>> CreateUser(CreateUserDto createUserDto);
}