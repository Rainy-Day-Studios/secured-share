using Microsoft.Extensions.Hosting;
using Infrastructure.Injection;
using UseCases.Injection;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace API
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureAppConfiguration(config =>
                    {
                        config.SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("local.settings.json", false, true);
                    })
                .ConfigureServices((context, services) =>
                {
                    services
                        .InjectInfrastructure(context.Configuration)
                        .InjectUseCases();
                })
                .Build();

            host.Run();
        }
    }
}