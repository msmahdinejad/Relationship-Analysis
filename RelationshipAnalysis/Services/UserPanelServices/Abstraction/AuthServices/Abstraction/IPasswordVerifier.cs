namespace RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices.Abstraction;

public interface IPasswordVerifier
{
    bool VerifyPasswordHash(string? password, string storedHash);
}