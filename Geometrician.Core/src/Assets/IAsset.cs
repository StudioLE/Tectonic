namespace Geometrician.Core.Assets;

/// <summary>
/// <para>
/// An information container is a uniquely identified document that contains information related to built assets.
/// The convention is defined in ISO 19650.
/// </para>
/// <para>
/// The metadata of the document.
/// This is loosely based on
/// <see href="https://standards.buildingsmart.org/IFC/RELEASE/IFC4_1/FINAL/HTML/link/ifcdocumentinformation.htm">IfcDocumentInformation</see>.
/// </para>
/// </summary>
public interface IAsset
{
    /// <summary>
    /// Identifier that uniquely identifies a document.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// File name or document name assigned by owner.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description of document and its content.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types">MIME type</see>.
    /// </summary>
    public string ContentType { get; set; }
}
