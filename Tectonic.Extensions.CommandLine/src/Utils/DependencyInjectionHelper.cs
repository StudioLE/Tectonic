using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudioLE.Extensions.Logging.Console;

namespace Tectonic.Extensions.CommandLine.Utils;

public static class DependencyInjectionHelper
{

    public static IHostBuilder RegisterCustomLoggingProviders(this IHostBuilder builder)
    {
        return builder
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddDebug();
                // logging.AddConsole();
                logging.AddBasicConsole();
            });
    }
}
