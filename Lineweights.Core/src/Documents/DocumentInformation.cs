namespace Lineweights.Core.Documents;

/// <summary>
/// The metadata of the document.
/// This is loosely based on
/// <see href="https://standards.buildingsmart.org/IFC/RELEASE/IFC4_1/FINAL/HTML/link/ifcdocumentinformation.htm">IfcDocumentInformation</see>.
/// </summary>
public class DocumentInformation : Element, ICloneable
{
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

    /// <inheritdoc />
    public object Clone()
    {
        return new DocumentInformation
        {
            Description = Description,
            Location = Location is not null
                ? new(Location.OriginalString)
                : null,
            CreationTime = CreationTime,
            LastRevisionTime = LastRevisionTime
        };
    }
}
