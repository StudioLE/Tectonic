using System.Text;

namespace Tectonic.Assets;

/// <summary>
/// Methods to help with <see cref="IAssetFileInfo"/>.
/// </summary>
public static class AssetFileInfoHelpers
{
    /// <summary>
    /// Determine if the asset includes inline data encoded as a data URI.
    /// </summary>
    /// <param name="asset">The asset.</param>
    /// <returns>
    /// <see langword="true"/> if the asset location is a data uri; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsInlineData(this IAssetFileInfo asset)
    {
        return asset.Location.StartsWith("data:");
    }

    /// <summary>
    /// Determine if the asset is a local file.
    /// </summary>
    /// <param name="asset">The asset.</param>
    /// <returns>
    /// <see langword="true"/> if the asset location is a data uri; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsLocalFile(this IAssetFileInfo asset)
    {
        return asset.Location.StartsWith("file:");
    }

    /// <summary>
    /// Get the inline data from the asset's data URI.
    /// </summary>
    /// <param name="asset">The asset.</param>
    /// <returns>
    /// <see langword="true"/> if the asset location is a data uri; otherwise, <see langword="false"/>.
    /// </returns>
    public static string GetInlineData(this IAssetFileInfo asset)
    {
        if (!IsInlineData(asset))
            return string.Empty;
        string[] parts = asset.Location.Split(',');
        string metadata = parts[0];
        string data = string.Join("", parts.Skip(1));
        if (!metadata.Contains(";base64"))
            return data;
        return Encoding.UTF8.GetString(Convert.FromBase64String(data));
    }

    /// <summary>
    /// Get the inline data from the asset's data URI.
    /// </summary>
    /// <param name="asset">The asset.</param>
    /// <returns>
    /// <see langword="true"/> if the asset location is a data uri; otherwise, <see langword="false"/>.
    /// </returns>
    public static byte[] GetInlineDataBytes(this IAssetFileInfo asset)
    {
        if (!IsInlineData(asset))
            return Array.Empty<byte>();
        string[] parts = asset.Location.Split(',');
        string metadata = parts[0];
        string data = string.Join("", parts.Skip(1));
        if (!metadata.Contains(";base64"))
            return Encoding.UTF8.GetBytes(data);;
        return Convert.FromBase64String(data);
    }
}
