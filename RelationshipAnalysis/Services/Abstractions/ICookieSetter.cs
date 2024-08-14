using Microsoft.Extensions.Options;

namespace RelationshipAnalysis.Services.Abstractions;

public interface ICookieSetter
{
    void SetCookie(HttpResponse response, string token);
}