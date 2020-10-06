using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace dotnet {
    public class Program {

  public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
        .AddEnvironmentVariables()
        .Build();


     public static void Main(string[] args) {

            var hostBuilder = CreateHostBuilder(args);
            var windowsService = bool.Parse(Configuration["WindowsService"]);

            if (windowsService) {
              hostBuilder.UseWindowsService();
            }
            var host = hostBuilder.Build();
            host.Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => {
                    services.AddHostedService<Worker>();
                });
    }
}
