using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RelationshipAnalysis.Models.Auth;

[Index(nameof(UserId), nameof(RoleId), IsUnique = true)]
public class UserRole
{
    [Key] public int Id { get; set; }

    public int UserId { get; set; }
    [ForeignKey("UserId")] public virtual User User { get; set; }

    public int RoleId { get; set; }
    [ForeignKey("RoleId")] public virtual Role Role { get; set; }
}