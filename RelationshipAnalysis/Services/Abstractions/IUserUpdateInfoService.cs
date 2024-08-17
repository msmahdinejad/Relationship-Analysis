using System.Security.Claims;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Models;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IUserUpdateInfoService
{
    Task<ActionResponse<MessageDto>> UpdateUserAsync(User user, UserUpdateInfoDto userUpdateInfoDto,
        HttpResponse response);
}