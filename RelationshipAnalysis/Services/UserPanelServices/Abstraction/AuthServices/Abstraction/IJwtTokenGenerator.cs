using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices.Abstraction;

public interface IJwtTokenGenerator
{
    string GenerateJwtToken(User user);
}