namespace StudioLE.Core.Exceptions;

/// <summary>
/// An error which occurs when the value of an <see cref="Enum"/> is not handled by a switch.
/// </summary>
public sealed class EnumSwitchException<T> : Exception where T : Enum
{
    /// <inheritdoc cref="EnumSwitchException{T}"/>
    public EnumSwitchException(string contextMessage, T value)
        : base(contextMessage + $" Unhandled enum value ({value}) of {nameof(T)}.")
    {
    }
}
