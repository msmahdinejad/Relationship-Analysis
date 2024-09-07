using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.UserDeleteService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.UserDeleteService;

public class UserDeleteServiceValidator(IMessageResponseCreator messageResponseCreator) : IUserDeleteServiceValidator
{
    public Task<ActionResponse<MessageDto>> Validate(User user)
    {
        if (user is null)
            return Task.FromResult(
                messageResponseCreator.Create(StatusCodeType.NotFound, Resources.UserNotFoundMessage));

        return Task.FromResult(messageResponseCreator.Create(StatusCodeType.Success,
            Resources.SuccessfulDeleteUserMessage));
    }
}