using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RelationshipAnalysis.Models.Graph;

[Index(nameof(EdgeAttributeName), IsUnique = true)]
public class EdgeAttribute
{
    [Key]
    public int EdgeAttributeId { get; set; }
    
    [Required] public string EdgeAttributeName { get; set; }

    
    public virtual ICollection<EdgeValue> EdgeValues { get; set; }
}
