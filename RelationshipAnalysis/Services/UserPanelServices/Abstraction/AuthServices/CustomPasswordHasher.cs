using System.Security.Cryptography;
using System.Text;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices.Abstraction;

namespace RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices;

public class CustomPasswordHasher : IPasswordHasher
{
    public string HashPassword(string? input)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        return hash;
    }
}