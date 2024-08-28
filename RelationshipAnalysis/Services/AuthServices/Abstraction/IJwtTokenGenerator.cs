using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.AuthServices.Abstraction;

public interface IJwtTokenGenerator
{
    string GenerateJwtToken(User user);
}