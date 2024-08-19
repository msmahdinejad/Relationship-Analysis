namespace RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices.Abstraction;

public interface IPasswordHasher
{
    string HashPassword(string? input);
}