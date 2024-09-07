using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RelationshipAnalysis.Context;

namespace RelationAnalysis.Migrations;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Run console app!");


        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json");
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;
                var connectionString = configuration.GetValue<string>("CONNECTION_STRING");

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(connectionString));

                services.AddTransient<InitialRecordsCreator>();
            })
            .Build();

        var configuration = host.Services.GetRequiredService<IConfiguration>();
        Console.WriteLine(configuration.GetValue<string>("CONNECTION_STRING"));
        Console.WriteLine(configuration.GetValue<string>("DefaultPassword"));

        using (var scope = host.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();

            var myService = scope.ServiceProvider.GetRequiredService<InitialRecordsCreator>();
            await myService.AddInitialRecords();
        }

        Console.WriteLine("Done");
    }
}