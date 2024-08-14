using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class PasswordVerifier(IPasswordHasher passwordHasher) : IPasswordVerifier
{
    public bool VerifyPasswordHash(string password, string storedHash)
    {
        return passwordHasher.HashPassword(password) == storedHash;
    }
}