using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Middlewares;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.AuthServices;
using RelationshipAnalysis.Services.AuthServices.Abstraction;
using RelationshipAnalysis.Services.CRUD.Permissions;
using RelationshipAnalysis.Services.CRUD.Permissions.Abstraction;
using RelationshipAnalysis.Services.CRUD.Role;
using RelationshipAnalysis.Services.CRUD.Role.Abstraction;
using RelationshipAnalysis.Services.CRUD.User;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;
using RelationshipAnalysis.Services.CRUD.UserRole;
using RelationshipAnalysis.Services.CRUD.UserRole.Abstraction;
using RelationshipAnalysis.Services.GraphServices;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Edge;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Graph;
using RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Node;
using RelationshipAnalysis.Services.GraphServices.Node.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.CreateUserService;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.CreateUserService.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.UserDeleteService;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.UserDeleteService.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.UserUpdateRolesService;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.UserUpdateRolesService.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.LogoutService;
using RelationshipAnalysis.Services.Panel.UserPanelServices.LogoutService.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.PermissionsService;
using RelationshipAnalysis.Services.Panel.UserPanelServices.PermissionsService.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserInfoService.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdatePasswordService;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdatePasswordService.Abstraction;
using RelationshipAnalysis.Settings.Authentication;
using RelationshipAnalysis.Settings.DbContext;
using RelationshipAnalysis.Settings.Services;
using EdgeAttributesReceiver = RelationshipAnalysis.Services.GraphServices.Edge.EdgeAttributesReceiver;
using NodeAttributesReceiver = RelationshipAnalysis.Services.GraphServices.Node.NodeAttributesReceiver;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddCustomServices();


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddAutoMapper(typeof(UserUpdateInfoMapper));

builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors(x => x.AllowCredentials().AllowAnyHeader().AllowAnyMethod()
    .SetIsOriginAllowed(x => true));
app.UseMiddleware<SanitizationMiddleware>();
app.Run();

namespace RelationshipAnalysis
{
    public class Program
    {
    }
}