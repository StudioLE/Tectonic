using System.IO;
using System.Reflection;
using Geometrician.Core.Execution;
using Microsoft.Extensions.Logging;
using StudioLE.Core.Results;
using StudioLE.Core.System;

namespace Geometrician.Cli;

/// <summary>
/// Run an <see cref="ActivityCommand"/>.
/// </summary>
/// <remarks>
/// The <see cref="ActivityCommand"/> is obtained using an <see cref="IActivityFactory"/>.
/// </remarks>
public sealed class RunCommand
{
    private readonly ILogger<RunCommand> _logger;
    private readonly IActivityFactory _factory;

    /// <inheritdoc cref="RunCommand"/>
    public RunCommand(ILogger<RunCommand> logger, IActivityFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    /// <summary>
    /// <inheritdoc cref="RunCommand"/>
    /// </summary>
    /// <param name="assemblyPath">The path to the assembly.</param>
    /// <param name="activity">The key of the activity.</param>
    /// <param name="inputsPath">The path to a the inputs as a json file.</param>
    /// <returns>A message describing the status.</returns>
    public string Execute(string assemblyPath, string activity, string inputsPath)
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

        IResult<ActivityCommand> result = _factory.TryCreateByKey(assembly, activity);

        if (result is not Success<ActivityCommand> success)
            return "Failed: " + result.Errors.Join(". ");

        // TODO: This would make so much more sense as a workflow and we get the IVisualizeStrategy via DI.
        object outputs = success.Value.Execute();

        return "Execution complete..";
    }
}
