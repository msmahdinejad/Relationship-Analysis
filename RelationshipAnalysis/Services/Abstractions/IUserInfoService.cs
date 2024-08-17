using System.Security.Claims;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IUserInfoService
{
    ActionResponse<UserOutputInfoDto> GetUser(User user);
}