namespace Tectonic.Assets;

/// <summary>
/// A uniquely identified document.
/// </summary>
/// <remarks>
/// The metadata of the document is loosely based on the information container of ISO 19650 and the IFC schema
/// <see href="https://standards.buildingsmart.org/IFC/RELEASE/IFC4_1/FINAL/HTML/link/ifcdocumentinformation.htm">IfcDocumentInformation</see>.
/// </remarks>
public interface IAsset
{
    /// <summary>
    /// Identifier that uniquely identifies a document.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The description.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// The <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types">MIME type</see>.
    /// </summary>
    public string ContentType { get; init; }
}
