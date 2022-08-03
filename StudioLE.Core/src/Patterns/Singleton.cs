namespace StudioLE.Core.Patterns;

/// <summary>
/// Create <typeparamref name="T"/> via a
/// <see href="https://refactoring.guru/design-patterns/singleton">singleton pattern</see>.
/// </summary>
/// <remarks>
/// This implementation is based on the example from
/// <see href="https://refactoring.guru/design-patterns/singleton/csharp/example#example-1">Refactoring Guru</see>.
/// </remarks>
public static class Singleton<T>
{
    // ReSharper disable once InconsistentNaming
    private static T? _instance;

    // We now have a lock object that will be used to synchronize threads
    // during first access to the Singleton.
    // ReSharper disable once InconsistentNaming
    private static readonly object _lock = new();

    /// <inheritdoc cref="Singleton{T}"/>
    public static T GetInstance(Func<T> constructor)
    {
        // This conditional is needed to prevent threads stumbling over the
        // lock once the instance is ready.
        if (_instance is not null)
            return _instance;

        // Now, imagine that the program has just been launched. Since
        // there's no Singleton instance yet, multiple threads can
        // simultaneously pass the previous conditional and reach this
        // point almost at the same time. The first of them will acquire
        // lock and will proceed further, while the rest will wait here.
        lock (_lock)
        {
            // The first thread to acquire the lock, reaches this
            // conditional, goes inside and creates the Singleton
            // instance. Once it leaves the lock block, a thread that
            // might have been waiting for the lock release may then
            // enter this section. But since the Singleton field is
            // already initialized, the thread won't create a new
            // object.
            _instance ??= constructor();
        }

        return _instance;
    }
}
