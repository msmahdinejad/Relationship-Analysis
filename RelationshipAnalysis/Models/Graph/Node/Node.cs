using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RelationshipAnalysis.Models.Graph.Node;

[Index(nameof(NodeUniqueString), nameof(NodeCategoryId), IsUnique = true)]
public class Node
{
    [Key] public int NodeId { get; set; }

    [Required] public string NodeUniqueString { get; set; }

    [ForeignKey("NodeCategory")] public int NodeCategoryId { get; set; }


    public virtual NodeCategory NodeCategory { get; set; }


    public virtual ICollection<NodeValue> Values { get; set; } = new List<NodeValue>();


    public virtual ICollection<Edge.Edge> SourceEdges { get; set; } = new List<Edge.Edge>();
    public virtual ICollection<Edge.Edge> DestinationEdges { get; set; } = new List<Edge.Edge>();
}