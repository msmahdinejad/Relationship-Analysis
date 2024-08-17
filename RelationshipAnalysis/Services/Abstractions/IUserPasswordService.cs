using System.Security.Claims;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Models;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IUserPasswordService
{
    Task<ActionResponse<MessageDto>> UpdatePasswordAsync(User user,
        UserPasswordInfoDto passwordInfoDto);
}