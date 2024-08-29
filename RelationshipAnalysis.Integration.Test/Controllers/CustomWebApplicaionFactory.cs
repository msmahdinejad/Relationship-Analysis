using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Node;

namespace RelationshipAnalysis.Integration.Test.Controllers;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null) services.Remove(descriptor);


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName)
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .UseLazyLoadingProxies();
            });


            var serviceProvider = services.BuildServiceProvider();


            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var dbContext = scopedServices.GetRequiredService<ApplicationDbContext>();


            dbContext.Database.EnsureCreated();
            SeedDatabase(dbContext);
        });
    }

    private void SeedDatabase(ApplicationDbContext dbContext)
    {
        var user = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "74b2c5bd3a8de69c8c7c643e8b5c49d6552dc636aeb0995aff6f01a1f661a979",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com"
        };
        var user2 = new User
        {
            Id = 2,
            Username = "admin2",
            PasswordHash = "74b2c5bd3a8de69c8c7c643e8b5c49d6552dc636aeb0995aff6f01a1f661a979",
            FirstName = "Admin2",
            LastName = "User2",
            Email = "admin2@example.com"
        };
        var role = new Role
        {
            Id = 1,
            Name = "Admin",
            Permissions = "[\"AdminPermissions\"]"
        };
        var userRole = new UserRole
        {
            Id = 1,
            RoleId = 1,
            Role = role,
            UserId = 1,
            User = user
        };
        dbContext.UserRoles.Add(userRole);
        user.UserRoles.Add(userRole);
        role.UserRoles.Add(userRole);
        dbContext.Users.Add(user);
        dbContext.Users.Add(user2);
        dbContext.Roles.Add(role);
        dbContext.SaveChanges();

        var nodeCategory1 = new NodeCategory
        {
            NodeCategoryId = 1,
            NodeCategoryName = "Account"
        };
        var nodeCategory2 = new NodeCategory
        {
            NodeCategoryId = 2,
            NodeCategoryName = "Person"
        };

        var node1 = new Node
        {
            NodeId = 1,
            NodeUniqueString = "Node1",
            NodeCategory = nodeCategory1,
            NodeCategoryId = nodeCategory1.NodeCategoryId
        };

        var node2 = new Node
        {
            NodeId = 2,
            NodeUniqueString = "Node2",
            NodeCategory = nodeCategory2,
            NodeCategoryId = nodeCategory2.NodeCategoryId
        };

        var edgeCategory = new EdgeCategory
        {
            EdgeCategoryId = 1,
            EdgeCategoryName = "Transaction"
        };


        dbContext.NodeCategories.Add(nodeCategory1);
        dbContext.NodeCategories.Add(nodeCategory2);
        dbContext.Nodes.Add(node1);
        dbContext.Nodes.Add(node2);
        dbContext.EdgeCategories.Add(edgeCategory);


        var edge = new Edge
        {
            EdgeId = 1,
            EdgeSourceNodeId = node1.NodeId,
            EdgeDestinationNodeId = node2.NodeId,
            EdgeCategory = edgeCategory,
            EdgeCategoryId = edgeCategory.EdgeCategoryId,
            EdgeUniqueString = "Edge1"
        };

        dbContext.Edges.Add(edge);

        dbContext.SaveChangesAsync();
    }
}