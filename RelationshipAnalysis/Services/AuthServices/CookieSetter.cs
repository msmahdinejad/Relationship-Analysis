using Microsoft.Extensions.Options;
using RelationshipAnalysis.Services.AuthServices.Abstraction;
using RelationshipAnalysis.Settings.JWT;

namespace RelationshipAnalysis.Services.AuthServices;

public class CookieSetter(IOptions<JwtSettings> jwtSettings) : ICookieSetter
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public void SetCookie(HttpResponse response, string token)
    {
        response.Cookies.Append(_jwtSettings.CookieName, token, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Secure = true,
            Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes)
        });
    }
}