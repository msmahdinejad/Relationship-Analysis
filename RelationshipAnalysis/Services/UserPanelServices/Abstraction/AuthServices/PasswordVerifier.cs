using RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices.Abstraction;

namespace RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices;

public class PasswordVerifier(IPasswordHasher passwordHasher) : IPasswordVerifier
{
    public bool VerifyPasswordHash(string password, string storedHash)
    {
        return passwordHasher.HashPassword(password) == storedHash;
    }
}