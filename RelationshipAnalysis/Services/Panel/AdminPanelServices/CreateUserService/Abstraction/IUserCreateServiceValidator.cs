using System.Threading.Tasks;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.Admin;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.CreateUserService.Abstraction;

public interface IUserCreateServiceValidator
{
    Task<ActionResponse<MessageDto>> Validate(CreateUserDto createUserDto);
}