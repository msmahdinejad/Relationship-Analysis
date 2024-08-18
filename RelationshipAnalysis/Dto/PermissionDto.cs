namespace RelationshipAnalysis.Dto;

public class PermissionDto
{
    public string Permissions { get; set; }

    public PermissionDto(string permissions)
    {
        Permissions = permissions;
    }
}