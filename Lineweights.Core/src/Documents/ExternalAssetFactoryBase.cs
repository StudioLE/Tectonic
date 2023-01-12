using StudioLE.Core.Conversion;
using StudioLE.Core.Results;

namespace Lineweights.Core.Documents;

public abstract class ExternalAssetFactoryBase<TSource> : IAssetFactory<TSource, ExternalAsset>
{
    private TSource? _source;

    /// <inheritdoc />
    protected abstract IConverter<TSource, Task<IResult<Uri>>> Converter { get; }

    /// <inheritdoc />
    public ExternalAsset Asset { get; } = new();

    /// <inheritdoc />
    public IResult Result { get; private set; } = new NotExecuted();

    /// <inheritdoc />
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
            throw new($"{nameof(_source)} is null.");
        IResult<Uri> result = await Converter.Convert(_source);
        if (result is Success<Uri> success)
        {
            Result = new Success();
            Asset.Location = success.Value;
        }
        else
            Result = new Failure();
    }
}
