using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RelationshipAnalysis.Models.Graph;

[Index(nameof(NodeUniqueString), nameof(NodeCategoryId), IsUnique = true)]

public class Node
{
    [Key]
    public int NodeId { get; set; }
    
    [Required] public string NodeUniqueString { get; set; }

    [ForeignKey("NodeCategory")]
    public int NodeCategoryId { get; set; }
    
    
    public virtual NodeCategory NodeCategory { get; set; }

    
    public virtual ICollection<NodeValue> Values { get; set; }

    
    public virtual ICollection<Edge> SourceEdges { get; set; }
    public virtual ICollection<Edge> DestinationEdges { get; set; }
}
