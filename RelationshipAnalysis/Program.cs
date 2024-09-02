using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
using RelationshipAnalysis.Settings.JWT;
using EdgeAttributesReceiver = RelationshipAnalysis.Services.GraphServices.Edge.EdgeAttributesReceiver;
using NodeAttributesReceiver = RelationshipAnalysis.Services.GraphServices.Node.NodeAttributesReceiver;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration["CONNECTION_STRING"]).UseLazyLoadingProxies());

builder.Services.AddSingleton<ICookieSetter, CookieSetter>()
    .AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>()
    .AddSingleton<ILoginService, LoginService>()
    .AddSingleton<IPermissionService, PermissionService>()
    .AddSingleton<IPasswordHasher, CustomPasswordHasher>()
    .AddSingleton<IAllUserService, AllUserService>()
    .AddSingleton<IUserUpdateInfoService, UserUpdateInfoService>()
    .AddSingleton<IUserDeleteService, UserDeleteService>()
    .AddSingleton<IUserReceiver, UserReceiver>()
    .AddSingleton<IUserUpdatePasswordService, UserUpdatePasswordService>()
    .AddSingleton<IUserInfoService, UserInfoService>()
    .AddSingleton<IPasswordVerifier, PasswordVerifier>()
    .AddSingleton<IRoleReceiver, RoleReceiver>()
    .AddSingleton<ILogoutService, LogoutService>()
    .AddSingleton<IUserCreateService, UserCreateService>()
    .AddSingleton<IUserUpdateRolesService, UserUpdateRolesService>()
    .AddSingleton<INodeCategoryReceiver, NodeCategoryReceiver>()
    .AddSingleton<IEdgeCategoryReceiver, EdgeCategoryReceiver>()
    .AddSingleton<ICreateNodeCategoryService, CreateNodeCategoryService>()
    .AddSingleton<ICreateEdgeCategoryService, CreateEdgeCategoryService>()
    .AddSingleton<IGraphReceiver, GraphReceiver>()
    .AddSingleton<IUserUpdateRolesService, UserUpdateRolesService>()
    .AddSingleton<IGraphReceiver, GraphReceiver>()
    .AddSingleton<INodesAdditionService, NodesAdditionService>()
    .AddSingleton<ISingleNodeAdditionService, SingleNodeAdditionService>()
    .AddSingleton<ICsvProcessorService, CsvProcessorService>()
    .AddSingleton<ISingleEdgeAdditionService, SingleEdgeAdditionService>()
    .AddSingleton<IEdgesAdditionService, EdgesAdditionService>()
    .AddSingleton<IMessageResponseCreator, MessageResponseCreator>()
    .AddSingleton<IUserUpdatePasswordServiceValidator, UserUpdatePasswordServiceValidator>()
    .AddSingleton<IUserUpdateInfoServiceValidator, UserUpdateInfoServiceValidator>()
    .AddSingleton<IUserOutputInfoDtoCreator, UserOutputInfoDtoCreator>()
    .AddSingleton<IUserInfoServiceValidator, UserInfoServiceValidator>()
    .AddSingleton<IUserUpdateRolesServiceValidator, UserUpdateRolesServiceValidator>()
    .AddSingleton<IUserDeleteServiceValidator, UserDeleteServiceValidator>()
    .AddSingleton<IUserCreateServiceValidator, UserCreateServiceValidator>()
    .AddSingleton<ICreateUserDtoMapper, CreateUserDtoMapper>()
    .AddSingleton<IAllUserServiceValidator, AllUserServiceValidator>()
    .AddSingleton<IAllUserDtoCreator, AllUserDtoCreator>()
    .AddSingleton<IPermissionsReceiver, PermissionsReceiver>()
    .AddSingleton<IRoleReceiver, RoleReceiver>()
    .AddSingleton<IUserAdder, UserAdder>()
    .AddSingleton<IUserDeleter, UserDeleter>()
    .AddSingleton<IUserReceiver, UserReceiver>()
    .AddSingleton<IUserUpdater, UserUpdater>()
    .AddSingleton<IUserRolesAdder, UserRolesAdder>()
    .AddSingleton<IUserRolesRemover, UserRolesRemover>()
    .AddSingleton<ICsvValidatorService, CsvValidatorService>()
    .AddSingleton<ICsvValidatorService, CsvValidatorService>()
    .AddSingleton<IGraphDtoCreator, GraphDtoCreator>()
    .AddSingleton<IContextNodesAdditionService, ContextNodesAdditionService>()
    .AddSingleton<INodeValueAdditionService, NodeValueAdditionService>()
    .AddSingleton<IEdgeValueAdditionService, EdgeValueAdditionService>()
    .AddSingleton<IContextEdgesAdditionService, ContextEdgesAdditionService>()
    .AddSingleton<IContextNodesAdditionService, ContextNodesAdditionService>()
    .AddSingleton<ICsvValidatorService, CsvValidatorService>()
    .AddSingleton<IExpansionGraphReceiver, ExpansionGraphReceiver>()
    .AddSingleton<IGraphDtoCreator, GraphDtoCreator>()
    .AddSingleton<IGraphSearcherService, GraphSearcherService>()
    .AddKeyedSingleton<IInfoReceiver, NodeInfoReceiver>("node")
    .AddKeyedSingleton<IInfoReceiver, EdgeInfoReceiver>("edge")
    .AddKeyedSingleton<IAttributesReceiver, NodeAttributesReceiver>("node")
    .AddKeyedSingleton<IAttributesReceiver, EdgeAttributesReceiver>("edge");
    


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddAutoMapper(typeof(UserUpdateInfoMapper));

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var cookie = context.Request.Cookies[jwtSettings.CookieName];
                if (!string.IsNullOrEmpty(cookie))
                {
                    context.Token = cookie;
                }
                return Task.CompletedTask;
            }
        };
    });


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
    public partial class Program
    {
    }
}