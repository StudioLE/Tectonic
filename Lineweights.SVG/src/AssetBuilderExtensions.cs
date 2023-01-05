using Lineweights.Core.Documents;
using Lineweights.Drawings;

namespace Lineweights.SVG;

/// <summary>
/// Methods to add SVG to <see cref="IAssetBuilder"/>.
/// </summary>
public static class AssetBuilderExtensions
{
    /// <inheritdoc cref="SvgAssetFactory{Sheet}"/>
    public static T ExtractSheetsAndConvertToSvg<T>(this T @this) where T : IAssetBuilder
    {
        SvgAssetFactory<Sheet> factory = new();
        @this.Factories.Add(factory);
        return @this;
    }

    /// <inheritdoc cref="SvgAssetFactory{View}"/>
    public static T ExtractViewsAndConvertToSvg<T>(this T @this) where T : IAssetBuilder
    {
        SvgAssetFactory<View> factory = new();
        @this.Factories.Add(factory);
        return @this;
    }
}
