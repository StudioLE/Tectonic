namespace StudioLE.Core.System;

/// <summary>
/// Methods to help with <see cref="Uri"/>.
/// </summary>
public static class UriHelpers
{
    /// <summary>
    /// Get the file name of <see cref="Uri"/>.
    /// </summary>
    public static string GetFileName(this Uri @this)
    {
        return @this.Segments.LastOrDefault() ?? throw new("Failed to GetFileName. The Uri had no segments.");
    }

    /// <summary>
    /// Get the file extension of <see cref="Uri"/>.
    /// </summary>
    public static string GetFileExtension(this Uri @this, string prefix = ".")
    {
        string fileName = @this.GetFileName();
        string fileExtension = fileName.Split('.').Last();
        return prefix + fileExtension;
    }
}
