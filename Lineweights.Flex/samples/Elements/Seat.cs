using Lineweights.Flex.Coordination;
using StudioLE.Core.System;

namespace Lineweights.Flex.Samples.Elements;

internal sealed class Seat : GeometricElement
{
    private const double DefaultWidth = .525;
    private const double DefaultCushionDepth = .650;
    private const double DefaultCushionHeight = .440;
    private const double DefaultBackDepth = .100;
    private const double DefaultBackHeight = .900;
    private const double DefaultSideSpacing = .050;
    private const double DefaultRowSpacing = .350;

    public Seat(string name)
    {
        Spacing spacing = new(DefaultSideSpacing, DefaultRowSpacing, 0);
        this.SetProperty(spacing);
        Name = name;
        Id = GuidHelpers.Create(typeof(Brick).GUID, name);
        IsElementDefinition = true;
        Material = MaterialByName("Aqua");
    }

    /// <inheritdoc/>
    public override void UpdateRepresentations()
    {
        Polygon cushionPolygon = Polygon.Rectangle(DefaultWidth, DefaultCushionDepth);
        Polygon backPolygon = Polygon
            .Rectangle(DefaultWidth, DefaultBackDepth)
            .TransformedPolygon(new(0, (DefaultCushionDepth - DefaultBackDepth) * 0.5, DefaultCushionHeight));
        Extrude cushion = new(cushionPolygon, DefaultCushionHeight, Vector3.ZAxis, false);
        Extrude back = new(backPolygon, DefaultBackHeight - DefaultCushionHeight, Vector3.ZAxis, false);
        Representation = new(cushion, back);
    }
}
