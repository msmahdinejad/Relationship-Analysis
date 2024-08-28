namespace RelationshipAnalysis.Dto.Panel.User;

public class PermissionDto
{
    public PermissionDto(string permissions)
    {
        Permissions = permissions;
    }

    public string Permissions { get; set; }
}