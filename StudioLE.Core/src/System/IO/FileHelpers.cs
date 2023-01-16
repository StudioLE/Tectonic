using System.IO;

namespace StudioLE.Core.System.IO;

/// <summary>
/// Methods to help with <see cref="FileInfo"/>.
/// </summary>
public static class FileHelpers
{
    /// <summary>
    /// Get the appropriate extension of the file according to its content type.
    /// </summary>
    public static string? GetExtensionByContentType(this string? contentType)
    {
        return contentType switch
        {
            "text/csv" => ".csv",
            "model/gltf-binary" => ".glb",
            "application/x-step" => ".ifc",
            "application/json" => ".json",
            "application/pdf" => ".pdf",
            "image/svg+xml" => ".svg",
            "text/plain" => ".txt",
            _ => null
        };
    }

    /// <summary>
    /// Get the appropriate content type of the file according to its extension.
    /// </summary>
    public static string? GetContentTypeByExtension(this FileInfo file)
    {
        return file.Extension switch
        {
            ".csv" => "text/csv",
            ".glb" => "model/gltf-binary",
            ".ifc" => "application/x-step",
            ".json" => "application/json",
            ".pdf" => "application/pdf",
            ".svg" => "image/svg+xml",
            ".txt" => "text/plain",
            _ => null
        };
    }

    /// <summary>
    /// Get the appropriate content type of the file according to its extension.
    /// </summary>
    public static string? GetContentTypeByExtension(this string fileName)
    {
        return new FileInfo(fileName).GetContentTypeByExtension();
    }
}
