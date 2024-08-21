using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Models.Graph;

namespace RelationshipAnalysis.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

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
}