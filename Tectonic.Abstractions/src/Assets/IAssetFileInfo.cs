namespace Tectonic.Assets;

/// <summary>
/// A uniquely identified document.
/// </summary>
/// <remarks>
/// The properties are based on the standards of ISO 19650:2018 and <c>IfcDocumentInformation</c>.
/// </remarks>
/// <seealso href="https://standards.buildingsmart.org/IFC/RELEASE/IFC4_1/FINAL/HTML/link/ifcdocumentinformation.htm"/>
public interface IAssetFileInfo
{
    /// <summary>
    /// Identifier that uniquely identifies a file.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The name.
    /// </summary>
    /// <remarks>
    /// Equivalent to the <c>Name</c> attribute of <c>IfcDocumentInformation</c>.
    /// </remarks>
    public string Name { get; init; }

    /// <summary>
    /// The description.
    /// </summary>
    /// <remarks>
    /// <para>Equivalent to the <c>Description</c> attribute of <c>IfcDocumentInformation</c>.</para>
    /// </remarks>
    /// <seealso href="https://standards.buildingsmart.org/IFC/RELEASE/IFC4_1/FINAL/HTML/link/ifcdocumentinformation.htm"/>
    public string Description { get; init; }

    /// <summary>
    /// The content, media, or MIME type.
    /// </summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types"/>
    public string MediaType { get; init; }

    /// <summary>
    /// The Uniform Resource Identifier (URI) or Uniform Resource Locator (URL) where the document can be retrieved.
    /// </summary>
    /// <remarks>
    /// <para>Where possible this should be a full URL including the scheme by which the file can be retrieved.</para>
    /// <para>For file paths it must be absolute, never relative.</para>
    /// <para>Equivalent to the <c>Location</c> attribute of <c>IfcDocumentInformation</c>.</para>
    /// </remarks>
    public string Location { get; init; }

    /// <summary>
    /// Does the file exist?
    /// </summary>
    /// <remarks>
    /// <para>Indicates whether the file existed when the <see cref="IAssetFileInfo"/> was created.</para>
    /// <para>Equivalent to the <c>IFileInfo.Exists</c> property.</para>
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.fileproviders.ifileinfo.exists?view=dotnet-plat-ext-8.0#microsoft-extensions-fileproviders-ifileinfo-exists"/>
    public bool Exists { get; init; }

    /// <summary>
    /// The optional container information.
    /// </summary>
    public IAssetContainerInfo? ContainerInformation { get; init; }
}
