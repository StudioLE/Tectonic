using StudioLE.Core.Results;

namespace Geometrician.Core.Assets;

/// <summary>
/// Create an <see cref="IAsset"/> of type <typeparamref name="TResult"/>.
/// </summary>
/// <remarks>
/// <para>
/// Follows a <see href="https://refactoring.guru/design-patterns/factory-method">factory method</see> design pattern.
/// </para>
/// <para>
/// The use of a factory enables the execution to be deferred until the result is required.
/// This is useful when the creation of the <see cref="IAsset"/> is expensive such as writing a PDF.
/// </para>
/// </remarks>
/// <typeparam name="TResult">The type of <see cref="IAsset"/> to create.</typeparam>
public interface IAssetFactory<out TResult> where TResult : IAsset
{
    /// <summary>
    /// The created <see cref="IAsset"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Implementations may set properties of <see cref="Asset"/> at any time; including from the constructor, from a setup method, or from <see cref="Execute"/>.
    /// </para>
    /// <para>
    /// This enables basic properties such as <see cref="IAsset.Name"/> and <see cref="IAsset.Description"/> to be set
    /// and read by the UI before the final asset is created.
    /// </para>
    /// </remarks>
    public TResult Asset { get; }

    /// <summary>
    /// The result of the execution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Typically if this is an instance of <see cref="NotExecuted"/> then <see cref="Execute"/> has not been called.
    /// </para>
    /// <para>
    /// Typically if this is an instance of <see cref="Success"/> then <see cref="Execute"/> has been called and <see cref="Asset"/> is now fully created.
    /// </para>
    /// <para>
    /// Typically if this is an instance of <see cref="Failure"/> then <see cref="Execute"/> has been called and <see cref="Asset"/> is now fully created.
    /// </para>
    /// </remarks>
    public IResult Result { get; }

    /// <summary>
    /// <inheritdoc cref="IAssetFactory{TResult}"/>.
    /// Set the result to <see cref="Asset"/> and update <see cref="Result"/> with the status.
    /// </summary>
    /// <returns>An awaitable <see cref="Task"/>.</returns>
    public Task Execute();
}

/// <summary>
/// Create an <see cref="IAsset"/> of type <typeparamref name="TResult"/> from <typeparamref name="TSource"/>.
/// </summary>
/// <remarks>
/// <inheritdoc cref="IAssetFactory{TResult}"/>
/// </remarks>
/// <typeparam name="TSource">The type to create from.</typeparam>
/// <typeparam name="TResult">The type of <see cref="IAsset"/> to create.</typeparam>
public interface IAssetFactory<in TSource, out TResult> : IAssetFactory<TResult> where TResult : IAsset
{
    /// <summary>
    /// Process the <paramref name="source"/> object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Typically implementations will extract any easily obtained information and store it to the properties of
    /// <see cref="IAssetFactory{TResult}.Asset"/>.
    /// </para>
    /// <para>
    /// Then they will store the <paramref name="source"/> as a private field to be used from the by the <see cref="IAssetFactory{TResult}.Execute"/>.
    /// </para>
    /// <para>
    /// However if the creation of the asset is simple then setup may immediately finalise creating the asset.
    /// </para>
    /// </remarks>
    /// <param name="source">The object to create an <see cref="IAsset"/> from.</param>
    public void Setup(TSource source);
}

/// <summary>
/// An <see cref="IResult"/> indicating that an <see cref="IAssetFactory{TResult}"/> has not been executed.
/// </summary>
/// <remarks>
/// <inheritdoc cref="IResult"/>
/// </remarks>
public class NotExecuted : IResult
{
    /// <inheritdoc/>
    public string[] Warnings { get; set; } = Array.Empty<string>();

    /// <inheritdoc/>
    public string[] Errors { get; } = Array.Empty<string>();
}
