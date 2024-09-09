using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;

namespace RelationshipAnalysis.Settings.DbContext;

public static class DbContextExtensions
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration["CONNECTION_STRING"]).UseLazyLoadingProxies());
            
        return services;
    }
}