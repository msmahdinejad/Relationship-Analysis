using DotNetEnv;
using DotNetEnv.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using Microsoft.Extensions.Configuration;

namespace RelationAnalysis.Migrations
{
    public class ConsoleStartup
    {
        public IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
        public ConsoleStartup()
        {
            
            //.. for test
            Console.WriteLine(Configuration["CONNECTION_STRING"]);
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(Configuration["CONNECTION_STRING"]).UseLazyLoadingProxies();
            });
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
       
        }
    }
}