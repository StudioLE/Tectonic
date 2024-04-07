using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudioLE.Extensions.Logging.Cache;
using StudioLE.Extensions.Logging.Console;

namespace Tectonic.Extensions.CommandLine.Tests.Resources;

public static class DependencyInjectionHelper
{
    public static IHostBuilder RegisterTestLoggingProviders(this IHostBuilder builder)
    {
        return builder
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddDebug();
                // logging.AddConsole();
                logging.AddBasicConsole();
                logging.AddCache();
            });
    }
}
