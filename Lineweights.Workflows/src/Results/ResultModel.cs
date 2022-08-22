namespace Lineweights.Workflows.Results;

/// <summary>
/// <para>
/// A base class for geometry tests and samples using the <see cref="Elements"/> library.
/// </para>
/// <para>
/// When executed in a DEBUG build the <see cref="Model"/> is serialised to json
/// and sent to via SignalR to the Dashboard to be visualised.
/// </para>
/// </summary>
/// <remarks>
/// <see cref="ResultModel"/> replicates the structure of Elements
/// <see href="https://github.com/hypar-io/Elements/blob/v1.0.0/Elements/test/ModelTest.cs">ModelTest</see>
/// class so that it can be used as a drop in replacement.
/// </remarks>
public class ResultModel
{
    #region Implement ModelTest

    /// <summary>
    /// The name.
    /// </summary>
    [Obsolete("A remnant of ModelTest that serves no purpose.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Should the model be serialised to the GLB format, saved, and then opened.
    /// </summary>
    [Obsolete("A remnant of ModelTest that serves no purpose.")]
    public bool GenerateGlb { get; set; } = false;

    /// <summary>
    /// Should the model be serialised to the GLB format, saved, and then opened.
    /// </summary>
    [Obsolete("A remnant of ModelTest that serves no purpose.")]
    public bool GenerateIfc { get; set; } = false;

    /// <summary>
    /// Should the model be serialised to the JSON format, saved, and then opened.
    /// </summary>
    [Obsolete("A remnant of ModelTest that serves no purpose.")]
    public bool GenerateJson { get; set; } = false;

    /// <summary>
    /// A test line.
    /// </summary>
    public static Line TestLine = new(Vector3.Origin, new(5, 5, 5));

    /// <summary>
    /// A test <see cref="Arc"/>.
    /// </summary>
    public static Arc TestArc = new(Vector3.Origin, 2.0, 0.0, 90.0);

    /// <summary>
    /// A test <see cref="Polyline"/>..
    /// </summary>
    public static Polyline TestPolyline = new(new Vector3(0, 0), new Vector3(0, 2), new Vector3(0, 3, 1));

    /// <summary>
    /// A test <see cref="Polygon"/>.
    /// </summary>
    public static Polygon TestPolygon = Polygon.Ngon(5, 2);

    /// <summary>
    /// A test <see cref="Circle"/>.
    /// </summary>
    public static Circle TestCircle = new(Vector3.Origin, 5);

    #endregion

    /// <summary>
    /// The <see cref="Model"/> containing all elements in the sample.
    /// </summary>
    public Model Model { get; private set; } = new();

    /// <summary>
    /// Replace the <see cref="Model"/> with a new copy.
    /// </summary>
    protected void ResetModel()
    {
        Model = new();
    }
}
