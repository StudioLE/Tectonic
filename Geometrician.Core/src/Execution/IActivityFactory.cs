using System.Reflection;
using StudioLE.Core.Results;

namespace Geometrician.Core.Execution;

/// <summary>
/// A <see href="https://refactoring.guru/design-patterns/factory-method">factory</see> to
/// create <see cref="IActivity"/>.
/// </summary>
public interface IActivityFactory
{
    /// <summary>
    /// Get the keys of all activities in the assembly.
    /// </summary>
    public IEnumerable<string> AllActivityKeysInAssembly(Assembly assembly);

    /// <summary>
    /// Create an <see cref="IActivity"/> by searching the assembly for an implementation
    /// that matches the key.
    /// </summary>
    public IResult<IActivity> TryCreateByKey(Assembly assembly, string activityKey);
}
