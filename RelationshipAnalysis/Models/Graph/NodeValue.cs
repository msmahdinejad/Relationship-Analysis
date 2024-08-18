using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RelationshipAnalysis.Models.Graph;

[Index(nameof(NodeId), nameof(NodeAttributeId), IsUnique = true)]
public class NodeValue
{
    [Key]
    public int ValueId { get; set; }

    [ForeignKey("Node")]
    public int NodeId { get; set; }

    [ForeignKey("NodeAttribute")]
    public int NodeAttributeId { get; set; }
    
    [Required] public string ValueData { get; set; }

    
    public virtual Node Node { get; set; }
    public virtual NodeAttribute NodeAttribute { get; set; }
}
