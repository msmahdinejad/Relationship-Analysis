namespace RelationshipAnalysis.Services.AuthServices.Abstraction;

public interface IPasswordVerifier
{
    bool VerifyPasswordHash(string? password, string storedHash);
}