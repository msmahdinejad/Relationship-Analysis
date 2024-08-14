using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.DTO;

namespace RelationshipAnalysis.Services.Abstractions;

public interface ILoginService
{
    Task<ActionResponse<MessageDto>> LoginAsync(LoginDto loginModel, HttpResponse response);
}