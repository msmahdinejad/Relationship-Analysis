using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services.Abstractions;
using RelationshipAnalysis.Settings.JWT;

namespace RelationshipAnalysis.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    private readonly SigningCredentials _creds;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        _creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        user.UserRoles.ToList().ForEach(ur =>
            claims.Add(new Claim(ClaimTypes.Role, ur.Role.Name)));

        var token = TokenGenerator(claims);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private SecurityToken TokenGenerator(List<Claim> claims)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
            SigningCredentials = _creds,
        };

        var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
        return token;
    }
}