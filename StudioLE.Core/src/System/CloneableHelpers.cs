namespace StudioLE.Core.System;

/// <summary>
/// Methods to help with <see cref="ICloneable"/>.
/// </summary>
public static class CloneableHelpers
{
    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// The clone is then case to <typeparamref name="T"/>.
    /// </summary>
    public static T CloneAs<T>(this T @this) where T : ICloneable
    {
        return (T)@this.Clone();
    }

    /// <summary>
    /// Create multiple clones of a given object.
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    public static IEnumerable<T> RepeatClone<T>(this T @this, int count) where T : ICloneable
    {
        return Enumerable
            .Range(0, count)
            .Select(_ => (T)@this.Clone());
    }
}
