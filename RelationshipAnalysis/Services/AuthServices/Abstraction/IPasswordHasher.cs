namespace RelationshipAnalysis.Services.AuthServices.Abstraction;

public interface IPasswordHasher
{
    string HashPassword(string? input);
}