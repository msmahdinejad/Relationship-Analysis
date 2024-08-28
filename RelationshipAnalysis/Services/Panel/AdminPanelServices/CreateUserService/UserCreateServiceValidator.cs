using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.CreateUserService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.CreateUserService;

public class UserCreateServiceValidator(
    IServiceProvider serviceProvider,
    IMessageResponseCreator messageResponseCreator) : IUserCreateServiceValidator
{
    public Task<ActionResponse<MessageDto>> Validate(CreateUserDto createUserDto)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (context.Users.Select(x => x.Username).ToList().Contains(createUserDto.Username))
        {
            return Task.FromResult(messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.UsernameExistsMessage));
        }
        
        if (context.Users.Select(x => x.Email).ToList().Contains(createUserDto.Email))
        {
            return Task.FromResult(messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.EmailExistsMessage));
        }

        if (createUserDto.Roles.IsNullOrEmpty())
        {
            return Task.FromResult(messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.EmptyRolesMessage));
        }

        var invalidRoles = createUserDto.Roles.FindAll(r => !context.Roles.Select(role => role.Name)
            .Contains(r));
        if (invalidRoles.Any())
        {
            return Task.FromResult(messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.InvalidRolesListMessage));
        }

        return Task.FromResult(messageResponseCreator.Create(StatusCodeType.Success, Resources.SucceddfulCreateUser));
    }
}