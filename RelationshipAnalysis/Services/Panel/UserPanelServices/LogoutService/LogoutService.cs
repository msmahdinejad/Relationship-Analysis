using Microsoft.AspNetCore.Http;
using RelationshipAnalysis.Services.Panel.UserPanelServices.LogoutService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.LogoutService;

public class LogoutService : ILogoutService
{
    public void Logout(HttpResponse response)
    {
        response.Cookies.Delete("jwt");
    }
}