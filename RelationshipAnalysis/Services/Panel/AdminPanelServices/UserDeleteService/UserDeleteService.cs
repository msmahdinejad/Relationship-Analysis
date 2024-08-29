using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.UserDeleteService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.UserDeleteService;

public class UserDeleteService(
    IUserDeleteServiceValidator validator,
    IUserDeleter userDeleter)
    : IUserDeleteService
{
    public async Task<ActionResponse<MessageDto>> DeleteUser(User user)
    {
        var validateResult = await validator.Validate(user);
        if (validateResult.StatusCode != StatusCodeType.Success) return validateResult;
        
        await userDeleter.DeleteUserAsync(user);

        return validateResult;
    }
}