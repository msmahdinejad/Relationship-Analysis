using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class LoginService(
    ApplicationDbContext context,
    ICookieSetter cookieSetter,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordVerifier passwordVerifier)
    : ILoginService
{
    public async Task<ActionResponse<MessageDto>> LoginAsync(LoginDto loginModel, HttpResponse response)
    {
        var result = new ActionResponse<MessageDto>();

        var user = await context.Users
            .SingleOrDefaultAsync(u => u.Username == loginModel.Username);

        if (user == null || !passwordVerifier.VerifyPasswordHash(loginModel.Password, user.PasswordHash))
        {
            result.Data = new MessageDto("Login failed!");
            result.StatusCode = StatusCodeType.Unauthorized;
            return result;
        }

        var token = jwtTokenGenerator.GenerateJwtToken(user);
        cookieSetter.SetCookie(response, token);

        result.Data = new MessageDto("Login was successful!");
        return result;
    }
}