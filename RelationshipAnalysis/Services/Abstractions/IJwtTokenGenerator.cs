using RelationshipAnalysis.Models;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateJwtToken(User user);
}