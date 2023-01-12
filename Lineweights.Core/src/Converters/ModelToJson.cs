using Lineweights.Core.Assets;
using StudioLE.Core.Conversion;
using StudioLE.Core.Results;

namespace Lineweights.Core.Converters;

/// <summary>
/// Convert a <see cref="Model"/> to JSON
/// referenced as <see cref="IAsset"/>.
/// </summary>
public class ModelToJson : IConverter<Model, Task<IResult<string>>>
{
    /// <inheritdoc />
    public Task<IResult<string>> Convert(Model model)
    {
        string json = model.ToJson();
        IResult<string> result = new Success<string>(json);
        return Task<IResult<string>>.FromResult(result);
    }
}
