namespace RelationshipAnalysis.Services.Abstractions;

public interface IPasswordVerifier
{
    bool VerifyPasswordHash(string password, string storedHash);
}