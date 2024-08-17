using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class LogoutService : ILogoutService
{
    public void Logout(HttpResponse response)
    {
        response.Cookies.Delete("jwt");
    }
}