using System.IO;
using System.Reflection;
using Cascade.Workflows;
using Microsoft.Extensions.Logging;
using StudioLE.Core.Results;
using StudioLE.Core.System;

namespace Cascade.Cli;

/// <summary>
/// Run an <see cref="IActivity"/>.
/// </summary>
/// <remarks>
/// The <see cref="IActivity"/> is obtained using an <see cref="IActivityResolver"/>.
/// </remarks>
public sealed class RunCommand
{
    private readonly ILogger<RunCommand> _logger;
    private readonly IActivityResolver _resolver;

    /// <inheritdoc cref="RunCommand"/>
    public RunCommand(ILogger<RunCommand> logger, IActivityResolver resolver)
    {
        _logger = logger;
        _resolver = resolver;
    }

    /// <summary>
    /// <inheritdoc cref="RunCommand"/>
    /// </summary>
    /// <param name="assemblyPath">The path to the assembly.</param>
    /// <param name="activity">The key of the activity.</param>
    /// <param name="inputsPath">The path to a the inputs as a json file.</param>
    /// <returns>A message describing the status.</returns>
    public async Task<string> Execute(string assemblyPath, string activity, string inputsPath)
    {
        _logger.LogDebug($"{nameof(Execute)} called.");
        _logger.LogDebug("Assembly: {$0}", assemblyPath);
        _logger.LogDebug("Activity: {$0}", activity);
        _logger.LogDebug("JSON: {$0}", inputsPath);

        assemblyPath = Path.GetFullPath(assemblyPath);

        if (!File.Exists(assemblyPath))
            return "Failed: The assembly does not exist.";

        Assembly assembly;
        try
        {
            assembly = Assembly.LoadFile(assemblyPath);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load assembly.");
            return $"Failed: {e.Message}";
        }

        IResult<IActivity> result = _resolver.Resolve(assembly, activity);

        if (result is not Success<IActivity> success)
            return "Failed: " + result.Errors.Join(". ");

        // TODO: This would make so much more sense as a workflow and we get the IVisualizeStrategy via DI.
        object outputs = await success.Value.Execute(null!);
        Console.WriteLine(outputs);
        return "Execution complete..";
    }
}
