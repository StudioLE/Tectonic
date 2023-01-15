using System.IO;
using System.Reflection;
using Geometrician.Core.Execution;
using Microsoft.Extensions.Logging;
using StudioLE.Core.System;

namespace Geometrician.Cli;

/// <summary>
/// List all <see cref="IActivity"/>.
/// </summary>
/// <remarks>
/// The <see cref="IActivity"/>s are obtained using an <see cref="IActivityFactory"/>.
/// </remarks>
public sealed class ListCommand
{
    private readonly ILogger<ListCommand> _logger;
    private readonly IActivityFactory _factory;

    /// <inheritdoc cref="ListCommand"/>
    public ListCommand(ILogger<ListCommand> logger, IActivityFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    /// <summary>
    /// <inheritdoc cref="ListCommand"/>
    /// </summary>
    /// <param name="assemblyPath">The path to the assembly.</param>
    /// <returns>A message describing the status.</returns>
    public string Execute(string assemblyPath)
    {
        _logger.LogDebug($"{nameof(Execute)} called.");
        _logger.LogDebug("Assembly: {$0}", assemblyPath);
        _logger.LogDebug("IActivityFactory: {$0}", _factory.GetType().ToString());

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
        IEnumerable<string> keys = _factory.AllActivityKeysInAssembly(assembly);
        return keys.Join();
    }
}
