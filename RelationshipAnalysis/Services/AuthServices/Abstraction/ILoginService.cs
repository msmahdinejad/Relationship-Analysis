using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Auth;
using LoginDto = RelationshipAnalysis.Dto.Auth.LoginDto;

namespace RelationshipAnalysis.Services.AuthServices.Abstraction;

public interface ILoginService
{
    Task<ActionResponse<MessageDto>> LoginAsync(LoginDto loginModel, HttpResponse response);
}