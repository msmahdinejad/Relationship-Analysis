using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RelationshipAnalysis.Models.Graph;

[Index(nameof(EdgeCategoryId), nameof(EdgeUniqueString), IsUnique = true)]
public class Edge
{
    [Key]
    public int EdgeId { get; set; }

    [ForeignKey("NodeSource")]
    public int EdgeSourceNodeId { get; set; }

    [ForeignKey("NodeDestination")]
    public int EdgeDestinationNodeId { get; set; }
    
    [ForeignKey("EdgeCategory")] 
    public int EdgeCategoryId { get; set; }
    
    [Required] public string EdgeUniqueString { get; set; }

    public virtual EdgeCategory EdgeCategory { get; set; }
    public virtual Node NodeSource { get; set; }
    public virtual Node NodeDestination { get; set; }
    public virtual ICollection<EdgeValue> EdgeValues { get; set; }
}
