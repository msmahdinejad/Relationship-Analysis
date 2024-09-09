namespace RelationshipAnalysis.Settings.Authentication;

public class JwtSettings
{
    public string CookieName { get; set; }
    public string Key { get; set; }
    public int ExpireMinutes { get; set; }
}