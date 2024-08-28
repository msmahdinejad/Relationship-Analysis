using Microsoft.Extensions.Configuration;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.AuthServices;

namespace RelationAnalysis.Migrations;

public class InitialRecordsCreator(ApplicationDbContext context, IConfiguration configuration)
{
    public async Task AddInitialRecords()
    {
        var roles = new List<Role>
        {
            new()
            {
                Name = "Admin",
                Permissions =
                    "[\"/api/Admin\",\"/api/Auth\", \"/api/User\", \"/api/Edge\", \"/api/Graph\", \"/api/Node\"]",
                Id = 1
            },
            new()
            {
                Name = "DataAdmin",
                Permissions = "[\"/api/Auth\", \"/api/User\", \"/api/Edge\", \"/api/Graph\", \"/api/Node\"]",
                Id = 2
            },
            new()
            {
                Name = "DataAnalyst",
                Permissions = "[\"/api/Auth\", \"/api/User\", \"/api/Edge\", \"/api/Graph\", \"/api/Node\"]",
                Id = 3
            }
        };
        var userRoles = new List<UserRole>
        {
            new()
            {
                UserId = 1,
                RoleId = 1
            },
            new()
            {
                UserId = 1,
                RoleId = 2
            },
            new()
            {
                UserId = 1,
                RoleId = 3
            }
        };
        var users = new List<User>
        {
            new()
            {
                Username = "admin",
                PasswordHash = new CustomPasswordHasher()
                    .HashPassword(configuration.GetValue<string>("DefaultPassword")),
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "admin@gmail.com",
                Id = 1
            }
        };
        try
        {
            await context.Roles.AddRangeAsync(roles);
            await context.Users.AddRangeAsync(users);
            await context.UserRoles.AddRangeAsync(userRoles);
            await context.SaveChangesAsync();
        }
        catch
        {
            Console.WriteLine("Data already exists!");
        }
    }
}