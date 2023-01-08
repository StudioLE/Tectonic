using Lineweights.Core.Documents;

namespace Lineweights.Workflows.Documents;

public static class AssetBuilderExtensions
{
    /// <inheritdoc cref="AssetFactory"/>
    public static T AddAssets<T>(this T @this, params Asset[] assets) where T : IAssetBuilder
    {
        AssetFactory factory = new(assets);
        @this.Factories.Add(factory);
        return @this;
    }

    /// <inheritdoc cref="GlbAssetFactory"/>
    public static T ConvertModelToGlb<T>(this T @this) where T : IAssetBuilder
    {
        GlbAssetFactory factory = new();
        @this.Factories.Add(factory);
        return @this;
    }

    /// <inheritdoc cref="JsonAssetFactory"/>
    public static T ConvertModelToJson<T>(this T @this) where T : IAssetBuilder
    {
        JsonAssetFactory factory = new();
        @this.Factories.Add(factory);
        return @this;
    }
}
