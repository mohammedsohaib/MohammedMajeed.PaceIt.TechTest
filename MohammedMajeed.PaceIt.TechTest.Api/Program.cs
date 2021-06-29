using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MohammedMajeed.PaceIt.TechTest.Data;

using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MohammedMajeed.PaceIt.TechTest.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                await DataContext.Initialise(scope.ServiceProvider, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Contacts.json"));
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
