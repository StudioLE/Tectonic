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
    private static T? _instance;
    private static readonly object _lock = new();

    /// <inheritdoc cref="Singleton{T}"/>
    public static T GetInstance(Func<T> constructor)
    {
        if (_instance is not null)
            return _instance;

        lock (_lock)
            _instance ??= constructor();

        return _instance;
    }
}
