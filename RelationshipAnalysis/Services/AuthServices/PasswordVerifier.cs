using RelationshipAnalysis.Services.AuthServices.Abstraction;

namespace RelationshipAnalysis.Services.AuthServices;

public class PasswordVerifier(IPasswordHasher passwordHasher) : IPasswordVerifier
{
    public bool VerifyPasswordHash(string? password, string storedHash)
    {
        return passwordHasher.HashPassword(password) == storedHash;
    }
}