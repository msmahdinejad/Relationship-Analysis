using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.UserDeleteService.Abstraction;

public interface IUserDeleteServiceValidator
{
    Task<ActionResponse<MessageDto>> Validate(User user);
}