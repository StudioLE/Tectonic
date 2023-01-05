using Lineweights.Core.Documents;

namespace Lineweights.IFC;

/// <summary>
/// Methods to add PDF to <see cref="IAssetBuilder"/>.
/// </summary>
public static class AssetBuilderExtensions
{
    /// <inheritdoc cref="Asset"/>
    public static T ConvertModelToIfc<T>(this T @this, DocumentInformation? doc = null) where T : IAssetBuilder
    {
        IfcAssetFactory factory = new(doc);
        @this.Factories.Add(factory);
        return @this;
    }
}
