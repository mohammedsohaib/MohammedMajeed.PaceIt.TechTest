using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MohammedMajeed.PaceIt.TechTest.Data;

namespace MohammedMajeed.PaceIt.TechTest.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                // Get the instance of DataContext in our services layer
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<DataContext>();

                // Call to create sample data
                DataContext.Initialise(services);
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
