using StudioLE.Core.Results;

namespace Lineweights.Core.Assets;

public interface IAssetFactory<out TResult> where TResult : IAsset
{
    public TResult Asset { get; }

    public IResult Result { get; }

    public Task Execute();
}

public interface IAssetFactory<in TSource, out TResult> : IAssetFactory<TResult> where TResult : IAsset
{
    public void Setup(TSource source);
}

public class NotExecuted : IResult
{
    /// <inheritdoc />
    public string[] Warnings { get; set; } = Array.Empty<string>();

    /// <inheritdoc />
    public string[] Errors { get; } = Array.Empty<string>();
}

