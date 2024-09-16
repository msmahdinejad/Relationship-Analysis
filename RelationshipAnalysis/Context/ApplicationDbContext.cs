using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Node;

namespace RelationshipAnalysis.Context;

public class ApplicationDbContext : DbContext
{

    public int LastNode => this.Nodes.Count() + this.ChangeTracker
        .Entries<Node>()
        .Count(e => e.State == EntityState.Added);

    public int LastNodeAttribute => this.NodeAttributes.Count() + this.ChangeTracker
        .Entries<NodeAttribute>()
        .Count(e => e.State == EntityState.Added);

    public int LastEdge => this.Edges.Count() + this.ChangeTracker
        .Entries<Edge>()
        .Count(e => e.State == EntityState.Added);

    public int LastEdgeAttribute=> this.EdgeAttributes.Count() + this.ChangeTracker
        .Entries<EdgeAttribute>()
        .Count(e => e.State == EntityState.Added);
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Edge> Edges { get; set; }
    public DbSet<EdgeAttribute> EdgeAttributes { get; set; }
    public DbSet<EdgeCategory> EdgeCategories { get; set; }
    public DbSet<EdgeValue> EdgeValues { get; set; }
    public DbSet<NodeAttribute> NodeAttributes { get; set; }
    public DbSet<NodeValue> NodeValues { get; set; }
    public DbSet<NodeCategory> NodeCategories { get; set; }
    public DbSet<Node> Nodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Edge>()
            .HasOne(e => e.NodeSource)
            .WithMany(n => n.SourceEdges)
            .HasForeignKey(e => e.EdgeSourceNodeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Edge>()
            .HasOne(e => e.NodeDestination)
            .WithMany(n => n.DestinationEdges)
            .HasForeignKey(e => e.EdgeDestinationNodeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}