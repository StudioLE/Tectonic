using Lineweights.Core.Documents;
using Lineweights.Drawings;

namespace Lineweights.PDF;

/// <summary>
/// Methods to add PDF to <see cref="IAssetBuilder"/>.
/// </summary>
public static class AssetBuilderExtensions
{
    /// <inheritdoc cref="PdfAssetFactory{Sheet}"/>
    public static T ExtractSheetsAndConvertToPdf<T>(this T @this) where T : IAssetBuilder
    {
        PdfAssetFactory<Sheet> factory = new();
        @this.Factories.Add(factory);
        return @this;
    }

    /// <inheritdoc cref="PdfAssetFactory{View}"/>
    public static T ExtractViewsAndConvertToPdf<T>(this T @this) where T : IAssetBuilder
    {
        PdfAssetFactory<View> factory = new();
        @this.Factories.Add(factory);
        return @this;
    }
}
