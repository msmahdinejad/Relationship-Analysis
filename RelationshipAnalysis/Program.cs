using AngleSharp.Common;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Middlewares;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService;
using RelationshipAnalysis.Settings.Authentication;
using RelationshipAnalysis.Settings.DbContext;
using RelationshipAnalysis.Settings.Services;

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