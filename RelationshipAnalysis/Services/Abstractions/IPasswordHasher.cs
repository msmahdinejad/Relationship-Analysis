namespace RelationshipAnalysis.Services.Abstractions;

public interface IPasswordHasher
{
    string HashPassword(string input);
}