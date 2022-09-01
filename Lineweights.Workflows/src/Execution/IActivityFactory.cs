using System.Reflection;
using Ardalis.Result;

namespace Lineweights.Workflows.Execution;

/// <summary>
/// A <see href="https://refactoring.guru/design-patterns/factory-method">factory</see> to
/// create <see cref="ActivityCommand"/>.
/// </summary>
public interface IActivityFactory
{
    /// <summary>
    /// Get the keys of all activities in the assembly.
    /// </summary>
    public IEnumerable<string> AllActivityKeysInAssembly(Assembly assembly);

    /// <summary>
    /// Create an <see cref="ActivityCommand"/> by searching the assembly for an implementation
    /// that matches the key.
    /// </summary>
    public Result<ActivityCommand> TryCreateByKey(Assembly assembly, string activityKey);
}
