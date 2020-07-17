using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace WhyNotEarth.Meredith.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddCommandLine(args)
                        .AddJsonFile("appsettings.local.json", true)
                        .Build());

                    webBuilder.ConfigureAppConfiguration((builderContext, config) =>
                    {
                        config.AddJsonFile("appsettings.local.json", true);
                    });

                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}