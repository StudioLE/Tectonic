using StudioLE.Core.Conversion;
using StudioLE.Core.Results;

namespace Lineweights.Core.Documents;

public abstract class InternalAssetFactoryBase<TSource> : IAssetFactory<TSource, InternalAsset>
{
    private TSource? _source;

    /// <inheritdoc />
    protected abstract IConverter<TSource, Task<IResult<string>>> Converter { get; }

    /// <inheritdoc />
    public InternalAsset Asset { get; } = new();

    /// <inheritdoc />
    public IResult Result { get; private set; } = new NotExecuted();

    public void Setup(TSource source)
    {
        _source = source;
        AfterSetup(source);
    }

    /// <inheritdoc />
    protected virtual void AfterSetup(TSource source)
    {
    }

    /// <inheritdoc />
    public async Task Execute()
    {
        if (Converter is null)
            throw new($"{nameof(Converter)} is null.");
        if (_source is null)
            throw new($"{nameof(Converter)} is null.");
        IResult<string> result = await Converter.Convert(_source);
        if (result is Success<string> success)
        {
            Result = new Success();
            Asset.Content = success.Value;
        }
        else
            Result = new Failure(result);
    }
}
