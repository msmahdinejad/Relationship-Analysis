using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RelationshipAnalysis.Models.Graph.Node;

[Index(nameof(NodeAttributeName), IsUnique = true)]
public class NodeAttribute
{
    [Key] public int NodeAttributeId { get; set; }

    [Required] public string NodeAttributeName { get; set; }


    public virtual ICollection<NodeValue> Values { get; set; } = new List<NodeValue>();
}