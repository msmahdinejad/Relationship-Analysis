using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RelationshipAnalysis.Models.Graph.Edge;

[Index(nameof(EdgeId), nameof(EdgeAttributeId), IsUnique = true)]
public class EdgeValue
{
    [Key] public int ValueId { get; set; }

    [ForeignKey("Edge")] public int EdgeId { get; set; }

    [ForeignKey("EdgeAttribute")] public int EdgeAttributeId { get; set; }

    [Required] public string ValueData { get; set; }

    public virtual Edge Edge { get; set; }
    public virtual EdgeAttribute EdgeAttribute { get; set; }
}