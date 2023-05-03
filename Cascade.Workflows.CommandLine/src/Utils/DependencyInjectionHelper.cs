using Cascade.Workflows.CommandLine.Utils.Logging.ColorConsoleLogger;
using Cascade.Workflows.CommandLine.Utils.Logging.TestLogger;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cascade.Workflows.CommandLine.Utils;

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
                logging.AddColorConsoleLogger();
            });
    }

    public static IHostBuilder RegisterTestLoggingProviders(this IHostBuilder builder)
    {
        return builder
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddDebug();
                // logging.AddConsole();
                logging.AddColorConsoleLogger();
                logging.AddTestLogger();
            });
    }
}
