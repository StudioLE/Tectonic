namespace StudioLE.Core.Conversion;

/// <summary>
/// Convert from a <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
/// </summary>
public interface IConverter<in TSource, out TResult>
{
    /// <summary>
    /// Convert from <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
    /// </summary>
    public TResult Convert(TSource source);
}
