namespace RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices.Abstraction;

public interface ICookieSetter
{
    void SetCookie(HttpResponse response, string token);
}