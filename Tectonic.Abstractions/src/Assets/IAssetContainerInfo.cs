namespace Tectonic.Assets;

/// <summary>
/// A uniquely identified document.
/// </summary>
/// <remarks>
/// The metadata of the document is loosely based on the information container of ISO 19650 and the IFC schema
/// <see href="https://standards.buildingsmart.org/IFC/RELEASE/IFC4_1/FINAL/HTML/link/ifcdocumentinformation.htm">IfcDocumentInformation</see>.
/// </remarks>
public interface IAssetContainerInfo
{
    /// <remarks>
    /// A single common project identifier should be defined at the initiation of the project. It should be
    /// independent and recognizably distinct from any individual organization's internal job number and be
    /// fixed within the project information standard. It is recommended that the code for the project field be
    /// between two and six characters in length.
    /// <para>There are no standard codes for the project field.</para>
    /// <para>A project can be divided into sub-projects.</para>
    /// <para>Where a project involves several elements or one element with several phases, each element or phase
    ///can be assigned an identifier.</para>
    /// <para>Equivalent to the <c>Project</c> field of <c>EN ISO 19650‑2:2018</c>.</para>
    /// </remarks>
    public string Project { get; init; }

    /// <remarks>
    /// A unique identifier should be defined for each organization on joining the project, to identify the
    /// organization responsible for producing the information within the container, and fixed within the
    /// project information standard. It is recommended that the code for the originator field be between three
    /// and six characters in length.
    /// <para>Where a project involves several elements or one element with several phases, each element or phase
    /// can be assigned an identifier.</para>
    /// <para>Equivalent to the <c>Originator</c> field of <c>EN ISO 19650‑2:2018</c>.</para>
    /// </remarks>
    public string Originator { get; init; }

    /// <summary>
    /// A unique identifier should be defined for each volume/system and fixed within the project information
    /// standard. It is recommended that the code for the volume/system field be two characters in length.
    /// The following standard codes should apply.
    /// </summary>
    /// <remarks>
    /// A unique identifier should be defined for each volume/system and fixed within the project information
    /// standard. It is recommended that the code for the volume/system field be two characters in length.
    /// The following standard codes should apply.
    /// Volume or System
    /// <para>A unique identifier should be defined for each volume/system and fixed within the project information
    /// standard. It is recommended that the code for the volume/system field be two characters in length.
    /// </para>
    /// <para>The following standard codes should apply.</para>
    /// <para>ZZ    all volumes/systems</para>
    /// <para>XX    no volume/system applicable</para>
    /// <para>ZZ    all volumes/systems</para>
    /// <para>This list can be expanded with project-specific codes.</para>
    /// <para>Equivalent to the <c>Volume/System</c> field of <c>EN ISO 19650‑2:2018</c>.</para>
    /// </remarks>
    public string VolumeOrSystem { get; init; }

    /// <remarks>
    /// A unique identifier should be defined for each level/location and fixed within the project information
    /// standard. It is recommended that the code for level/location field be two characters in length.
    /// <para>The following standard codes should apply.</para>
    /// <para>ZZ    multiple levels/locations</para>
    /// <para>XX    no level/location applicable</para>
    /// <para>00    base level</para>
    /// <para>01    level 01</para>
    /// <para>02    level 02, etc.</para>
    /// <para>M1    mezzanine above level 01</para>
    /// <para>M2    mezzanine above level 02, etc.</para>
    /// <para>B1    basement level 1</para>
    /// <para>B2    basement level 2, etc.</para>
    /// <para>Equivalent to the <c>level/Location</c> field of <c>EN ISO 19650‑2:2018</c>.</para>
    /// </remarks>
    public string LevelOrLocation { get; init; }

    /// <remarks>
    /// A unique identifier should be defined for each type of information, to identify the type of information
    /// held within the information container, and fixed within the project information standard. It is
    /// recommended that the code for the type field be two characters in length.
    /// <para>The following standard codes should apply.</para>
    /// <para>AF    animation file (of a model)</para>
    /// <para>CA    calculations</para>
    /// <para>BQ    bill of quantities</para>
    /// <para>CM    combined model (combined multidiscipline model)</para>
    /// <para>CO    correspondence</para>
    /// <para>CP    cost plan</para>
    /// <para>CR    clash rendition</para>
    /// <para>DB    database</para>
    /// <para>DR    drawing rendition</para>
    /// <para>FN    file note</para>
    /// <para>HS    health and safety</para>
    /// <para>IE    information exchange file</para>
    /// <para>M2    2D model</para>
    /// <para>M3    3D model</para>
    /// <para>MI    minutes / action notes</para>
    /// <para>MR    model rendition for other renditions, e.g. thermal analysis, etc.</para>
    /// <para>MS    method statement</para>
    /// <para>PP    presentation</para>
    /// <para>PR    programme</para>
    /// <para>RD    room data sheet</para>
    /// <para>RI    request for information</para>
    /// <para>RP    report</para>
    /// <para>SA    schedule of accommodation</para>
    /// <para>SH    schedule</para>
    /// <para>SN    snagging list</para>
    /// <para>SU    survey</para>
    /// <para>SP    specification</para>
    /// <para>VS    visualization</para>
    /// <para>This list can be expanded with project-specific codes.</para>
    /// <para>Equivalent to the <c>Type</c> field of <c>EN ISO 19650‑2:2018</c>.</para>
    /// </remarks>

    public string Type { get; init; }

    /// <remarks>
    /// A unique identifier should be defined for each role on the project that an organization is assigned and
    /// fixed within the project information standard. It is recommended that the code for the role field be one
    /// or two characters in length.
    /// <para>A    Architect</para>
    /// <para>B    Building surveyor</para>
    /// <para>C    civil engineer</para>
    /// <para>D    Drainage, highways engineer</para>
    /// <para>E    Electrical engineer</para>
    /// <para>F    Facilities manager</para>
    /// <para>G    Geographical and land surveyor</para>
    /// <para>H    Heating and ventilation designer (deprecated)</para>
    /// <para>I    Interior designer</para>
    /// <para>K    Client</para>
    /// <para>L    Landscape architect</para>
    /// <para>M    Mechanical engineer</para>
    /// <para>P    Public health engineer</para>
    /// <para>Q    Quantity surveyor</para>
    /// <para>S    Structural engineer</para>
    /// <para>T    Town and country planner</para>
    /// <para>W    Contractor</para>
    /// <para>X    Subcontractor</para>
    /// <para>Y    Specialist designer</para>
    /// <para>Z    General (non-disciplinary)</para>
    /// <para>This list can be expanded with two character project-specific codes.</para>
    /// <para>Equivalent to the <c>Role</c> field of <c>EN ISO 19650‑2:2018</c>.</para>
    /// </remarks>
    public string Role { get; init; }

    /// <remarks>
    /// A sequential number should be assigned to each information container when it is one of a series, not
    /// distinguished by any other of the fields.
    /// The numbering for standard coding should be fixed within the project information standard and it is
    /// recommended that it be between four and six integer numeric digits in length.
    /// <para>Equivalent to the <c>Number</c> field of <c>EN ISO 19650‑2:2018</c>.</para>
    /// </remarks>
    public string Number { get; init; }

    /// <remarks>
    /// A sequential number should be assigned to each information container when it is one of a series, not
    /// distinguished by any other of the fields.
    /// The numbering for standard coding should be fixed within the project information standard and it is
    /// recommended that it be between four and six integer numeric digits in length.
    /// <para>Work in progress (WIP)</para>
    /// <para>S0    Initial status</para>
    /// <para>Shared (non-contractual)</para>
    /// <para>S1    Suitable for coordination</para>
    /// <para>S2    Suitable for information</para>
    /// <para>S3    Suitable for review and comment</para>
    /// <para>S4    Suitable for stage approval</para>
    /// <para>S5    Withdrawn</para>
    /// <para>S6    Suitable for PIM authorization</para>
    /// <para>S7    Suitable for AIM authorization</para>
    /// <para>Published (contractual)</para>
    /// <para>A1, An, etc.    Authorized and accepted</para>
    /// <para>B1, Bn, etc.    Partial sign-off (with comments)</para>
    /// <para>Published (for AIM acceptance)</para>
    /// <para>CR    As constructed record document</para>
    /// <para>Equivalent to the <c>Project</c> field of <c>EN ISO 19650‑2:2018</c>.</para>
    /// </remarks>
    public string Status { get; init; }

    /// <remarks>
    /// <para>
    /// Preliminary revisions of information containers should be two integers, prefixed with the letter
    /// 'P', e.g. P01.
    /// </para>
    /// <para>
    /// Preliminary revisions of information containers in the 'work in progress' state should also have a two
    /// integer suffix to identify the version of the preliminary revision, e.g. P02.05.
    /// </para>
    /// <para>The initial revision of information containers should be P01.01.</para>
    /// <para>
    /// Contractual revisions of information containers should be two integers, prefixed with the letter
    /// 'C', e.g. C01.
    /// </para>
    /// <para>Equivalent to the <c>Revision</c> field of <c>EN ISO 19650‑2:2018</c>.</para>
    /// </remarks>
    public string Revision { get; init; }
}
