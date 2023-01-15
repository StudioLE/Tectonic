using Geometrician.Core.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static Towel.CommandLine;

namespace Geometrician.Cli;

/// <summary>
/// The entry point for the program.
/// </summary>
public static class Program
{
    private static IServiceProvider _services = null!;

    /// <summary>
    /// The entry point for the program.
    /// </summary>
    /// <param name="args">The program arguments.</param>
    public static void Main(string[] args)
    {
        IHostBuilder? builder = Host.CreateDefaultBuilder(args);
        builder.ConfigureServices((_, services) =>
        {
            services.AddLogging(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Debug));
            services.AddScoped<ListCommand>();
            services.AddScoped<RunCommand>();
            services.AddScoped<IActivityResolver, StaticMethodActivityResolver>();
        });

        using IHost? host = builder.Build();
        _services = host.Services;
        HandleArguments(args);
    }

    /// <summary>
    /// Get the keys of all activities in the assembly.
    /// </summary>
    /// <param name="assembly">The path to the assembly.</param>
    [Command]
    // ReSharper disable once InconsistentNaming
    public static void list(string assembly)
    {
        ListCommand command = _services.GetRequiredService<ListCommand>();
        string output = command.Execute(assembly);
        Console.WriteLine(output);
    }

    /// <summary>
    /// Run an activity.
    /// </summary>
    /// <param name="assembly">The path to the assembly.</param>
    /// <param name="activity">The key of the activity.</param>
    /// <param name="inputs">The path to a the inputs as a json file.</param>
    [Command]
    // ReSharper disable once InconsistentNaming
    public static void run(string assembly, string activity, string inputs = "")
    {
        RunCommand command = _services.GetRequiredService<RunCommand>();
        string output = command.Execute(assembly, activity, inputs);
        Console.WriteLine(output);
    }
}
