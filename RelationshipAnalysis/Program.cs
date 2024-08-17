using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.Abstractions;
using RelationshipAnalysis.Settings.JWT;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<ICookieSetter, CookieSetter>()
    .AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>()
    .AddScoped<ILoginService, LoginService>()
    .AddScoped<IPermissionService, PermissionService>()
    .AddSingleton<IPasswordHasher, CustomPasswordHasher>()
    .AddSingleton<IPasswordVerifier, PasswordVerifier>()
    .AddScoped<IAllUserService, AllUserService>()
    .AddScoped<IUserUpdateInfoService, UserUpdateInfoService>()
    .AddScoped<IUserDeleteService, UserDeleteService>()
    .AddScoped<IUserReceiver, UserReceiver>()
    .AddScoped<IUserPasswordService, UserPasswordService>()
    .AddScoped<IUserInfoService, UserInfoService>()
    .AddSingleton<IPasswordVerifier, PasswordVerifier>()
    .AddScoped<IRoleReceiver, RoleReceiver>()
    .AddSingleton<ILogoutService, LogoutService>()
    .AddScoped<IUserCreateService, UserCreateService>()
    .AddScoped<IUserUpdateRolesService, UserUpdateRolesService>()
    .AddScoped<IRoleReceiver, RoleReceiver>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql( Environment.GetEnvironmentVariable("CONNECTION_STRING")).UseLazyLoadingProxies());

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

app.Run();

public partial class Program
{
}