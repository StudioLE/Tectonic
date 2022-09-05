using Lineweights.Core.Distribution;
using StudioLE.Core.System;

namespace Lineweights.Flex;

internal class Brick : GeometricElement
{
    public BBox3 Bounds { get; }

    public Brick(BBox3 bounds)
    {
        Bounds = bounds;
    }

    public Brick(
        double width,
        double length,
        double height,
        double spacing,
        string name
    )
    {
        Bounds = CreateBBox3.ByLengths(width, length, height);
        this.SetProperty<Spacing>(new(spacing));
        Name = name;
        Id = GuidHelpers.Create(typeof(Brick).GUID, name);
        IsElementDefinition = true;
        Material = MaterialByName("Red");
    }

    /// <inheritdoc/>
    public override void UpdateRepresentations()
    {
        Representation = new(Bounds.ToExtrude());
    }

    public static Brick Stretcher => new(.215, .1025, .065, .01, "Stretcher");

    public static Brick Header => new(.1025, .215, .065, .01, "Header");

    public static Brick Half => new(.1025, .1075, .065, .01, "Half");

    public static Brick Soldier => new(.065, .1025, .215, .01, "Soldier");

    public static Brick Sailor => new(.1025, .065, .215, .01, "Sailor");
}
