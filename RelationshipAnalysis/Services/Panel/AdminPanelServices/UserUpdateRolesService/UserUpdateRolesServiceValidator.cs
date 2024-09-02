using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.UserUpdateRolesService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.UserUpdateRolesService;

public class UserUpdateRolesServiceValidator(
    IServiceProvider serviceProvider,
    IMessageResponseCreator messageResponseCreator) : IUserUpdateRolesServiceValidator
{
    public Task<ActionResponse<MessageDto>> Validate(User user, List<string> newRoles)
    {
        if (newRoles.IsNullOrEmpty())
            return Task.FromResult(messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.EmptyRolesMessage));

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var invalidRoles = newRoles.FindAll(r => !context.Roles.Select(role => role.Name)
            .Contains(r));
        if (invalidRoles.Any())
            return Task.FromResult(messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.InvalidRolesListMessage));
        
        return Task.FromResult(messageResponseCreator.Create(StatusCodeType.Success, Resources.SuccessfulUpdateRolesMessage));
    }
}