namespace StudioLE.Core.Exceptions;

/// <summary>
/// An error which occurs when a type ois not handled by a switch.
/// </summary>
public sealed class TypeSwitchException<T> : Exception
{
    /// <inheritdoc cref="TypeSwitchException{T}"/>
    public TypeSwitchException(string contextMessage, T value)
        : base(contextMessage + $" Unhandled type ({value?.GetType()}) of {nameof(T)}.")
    {
    }

    /// <inheritdoc cref="TypeSwitchException{T}"/>
    public TypeSwitchException(T value) : this(string.Empty, value)
    {
    }
}
