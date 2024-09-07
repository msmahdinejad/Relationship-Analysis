using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService;

public class UserUpdateInfoServiceValidator(
    IServiceProvider serviceProvider,
    IMessageResponseCreator messageResponseCreator) : IUserUpdateInfoServiceValidator
{
    public Task<ActionResponse<MessageDto>> Validate(User user, UserUpdateInfoDto userUpdateInfoDto)
    {
        if (user == null)
            return Task.FromResult(
                messageResponseCreator.Create(StatusCodeType.NotFound, Resources.UserNotFoundMessage));

        if (!IsUsernameUnique(user.Username, userUpdateInfoDto.Username))
            return Task.FromResult(messageResponseCreator.Create(StatusCodeType.BadRequest,
                Resources.UsernameExistsMessage));

        if (!IsEmailUnique(user.Email, userUpdateInfoDto.Email))
            return Task.FromResult(messageResponseCreator.Create(StatusCodeType.BadRequest,
                Resources.EmailExistsMessage));

        return Task.FromResult(messageResponseCreator.Create(StatusCodeType.Success,
            Resources.SuccessfulUpdateUserMessage));
    }

    private bool IsUsernameUnique(string currentValue, string newValue)
    {
        if (currentValue == newValue) return true;

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return !context.Users.Any(u => u.Username == newValue);
    }

    private bool IsEmailUnique(string currentValue, string newValue)
    {
        if (currentValue == newValue) return true;

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return !context.Users.Any(u => u.Email == newValue);
    }
}