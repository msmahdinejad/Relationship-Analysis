using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Auth;
using LoginDto = RelationshipAnalysis.Dto.Auth.LoginDto;

namespace RelationshipAnalysis.Services.AuthServices.Abstraction;

public interface ILoginService
{
    Task<ActionResponse<MessageDto>> LoginAsync(LoginDto loginModel, HttpResponse response);
}