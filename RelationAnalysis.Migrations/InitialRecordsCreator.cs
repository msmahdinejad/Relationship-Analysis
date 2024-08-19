using Microsoft.Extensions.Configuration;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices;

namespace RelationAnalysis.Migrations;

public class InitialRecordsCreator(ApplicationDbContext context, IConfiguration configuration)
{
    public async Task AddInitialRecords()
    {
        var roles = new List<Role>()
        {
            new Role()
            {
                Name = "Admin",
                Permissions =
                    "'[\"/api/Access/GetPermissions\",\"/api/Admin/GetUser/{id}\",\"/api/Admin/GetAllUser\",\"/api/Admin/GetAllRoles\",\"/api/Admin/UpdateUser/{id}\",\"/api/Admin/UpdatePassword/{id}\",\"/api/Admin/DeleteUser/{id}\",\"/api/Admin/CreateUser\",\"/api/Admin/UpdateRoles/{id}\",\"/api/Auth/Login\", \"/api/User/GetUser\",\"/api/User/UpdateUser\",\"/api/User/UpdatePassword\",\"/api/User/Logout\"]'",
                Id = 1
            },
            new Role()
            {
                Name = "DataAdmin",
                Permissions =
                    "'[\"/api/Access/GetPermissions\",\"/api/Auth/Login\",\"/api/User/GetUser\",\"/api/User/UpdateUser\",\"/api/User/UpdatePassword\",\"/api/User/Logout\"]'",
                Id = 2
            },
            new Role()
            {
                Name = "DataAnalyst",
                Permissions =
                    "'[\"/api/Access/GetPermissions\",\"/api/Auth/Login\",\"/api/User/GetUser\",\"/api/User/UpdateUser\",\"/api/User/UpdatePassword\",\"/api/User/Logout\"]'",
                Id = 3
            }
        };
        var userRoles = new List<UserRole>()
        {
            new UserRole()
            {
                UserId = 1,
                RoleId = 1
            },
            new UserRole()
            {
                UserId = 1,
                RoleId = 2
            },
            new UserRole()
            {
                UserId = 1,
                RoleId = 3
            }
        };
        var users = new List<User>()
        {
            new User()
            {
                Username = "admin",
                PasswordHash = new CustomPasswordHasher()
                    .HashPassword(configuration.GetValue<string>("DefaultPassword")),
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "admin@gmail.com",
                Id = 1,
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