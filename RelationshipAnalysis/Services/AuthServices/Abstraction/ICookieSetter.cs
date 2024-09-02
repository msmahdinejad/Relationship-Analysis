using Microsoft.AspNetCore.Http;

namespace RelationshipAnalysis.Services.AuthServices.Abstraction;

public interface ICookieSetter
{
    void SetCookie(HttpResponse response, string token);
}