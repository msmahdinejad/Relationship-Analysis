using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Auth;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.AuthServices.Abstraction;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;

namespace RelationshipAnalysis.Services.AuthServices;

public class LoginService(
    IMessageResponseCreator messageResponseCreator,
    IUserReceiver userReceiver,
    ICookieSetter cookieSetter,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordVerifier passwordVerifier)
    : ILoginService
{
    public async Task<ActionResponse<MessageDto>> LoginAsync(LoginDto loginModel, HttpResponse response)
    {
        var user = await userReceiver.ReceiveUserAsync(loginModel.Username);

        if (user == null || !passwordVerifier.VerifyPasswordHash(loginModel.Password, user.PasswordHash))
            return messageResponseCreator.Create(StatusCodeType.Unauthorized, Resources.LoginFailedMessage);

        var token = jwtTokenGenerator.GenerateJwtToken(user);
        cookieSetter.SetCookie(response, token);

        return messageResponseCreator.Create(StatusCodeType.Success, Resources.SuccessfulLoginMessage);
    }
}