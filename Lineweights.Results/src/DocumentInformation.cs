namespace Lineweights.Results;

/// <summary>
/// The metadata of the document.
/// This is loosely based on
/// <see href="https://standards.buildingsmart.org/IFC/RELEASE/IFC4_1/FINAL/HTML/link/ifcdocumentinformation.htm">IfcDocumentInformation</see>.
/// </summary>
public class DocumentInformation
{
    // TODO: Make DocumentInformation an Element as it's a property set. It can then be retrieved from the Model similar to linked files?
    /// <summary>
    /// The id.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// File name or document name assigned by owner..
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of document and its content.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Resource identifier or locator.
    /// </summary>
    public Uri? Location { get; set; } = null;

    /// <summary>
    /// Date and time stamp when the document was originally created. 
    /// </summary>
    public DateTime CreationTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Date and time stamp when this document version was created. 
    /// </summary>
    public DateTime LastRevisionTime { get; set; } = DateTime.Now;
}
