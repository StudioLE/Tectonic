using Lineweights.Flex.Sequences;
using Lineweights.Workflows.Results;

namespace Lineweights.Flex.Tests;

[SendToServerAfterTest]
internal sealed class Flex1dTests : ResultModel
{
    private Line Line { get; }
    private Vector3 CrossAxis { get; }
    private Arc Arc { get; }
    private SequenceBuilder StretcherSoldier { get; }
    private SequenceBuilder StretcherHeader { get; }

    public Flex1dTests()
    {
        const double length = 4;
        Line = new(Vector3.Origin, Vector3.XAxis, length);
        CrossAxis = Vector3.YAxis;
        Arc = new(3, 0, 90);
        StretcherSoldier = RepeatingSequence.MaxCount(20, Brick.Stretcher.CreateInstance(), Brick.Soldier.CreateInstance());
        StretcherHeader = RepeatingSequence.MaxCount(20, Brick.Stretcher.CreateInstance(), Brick.Header.CreateInstance());
    }

    [SetUp]
    public void Setup()
    {
        Model.AddElements(CreateModelArrows.ByTransform(new()));
    }

    [TestCase(Justification.Center)]
    [TestCase(Justification.End)]
    [TestCase(Justification.SpaceAround)]
    [TestCase(Justification.SpaceBetween)]
    [TestCase(Justification.SpaceEvenly)]
    [TestCase(Justification.Start)]
    public void Flex1d_Line_Justification_StretcherHeader(Justification justification)
    {
        // Arrange
        Flex1d builder = new Flex1d()
            .Bounds(Line, CrossAxis)
            .MainJustification(justification)
            .CrossAlignment(Alignment.Start)
            .CrossSettingOut(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .Pattern(StretcherHeader);
        ExecuteTest(builder);
    }

    [TestCase(Justification.Center)]
    [TestCase(Justification.End)]
    [TestCase(Justification.SpaceAround)]
    [TestCase(Justification.SpaceBetween)]
    [TestCase(Justification.SpaceEvenly)]
    [TestCase(Justification.Start)]
    public void Flex1d_Line_Justification_StretcherSoldier(Justification justification)
    {
        // Arrange
        Flex1d builder = new Flex1d()
            .Bounds(Line, CrossAxis)
            .MainJustification(justification)
            .CrossAlignment(Alignment.Start)
            .CrossSettingOut(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .Pattern(StretcherSoldier);
        ExecuteTest(builder);
    }

    [TestCase(Alignment.Center)]
    [TestCase(Alignment.End)]
    [TestCase(Alignment.Start)]
    public void Flex1d_Line_CrossAlignment_StretcherHeader(Alignment alignment)
    {
        // Arrange
        Flex1d builder = new Flex1d()
            .Bounds(Line, CrossAxis)
            .MainJustification(Justification.SpaceEvenly)
            .CrossAlignment(alignment)
            .CrossSettingOut(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .Pattern(StretcherHeader);
        ExecuteTest(builder);
    }

    [TestCase(Alignment.Center)]
    [TestCase(Alignment.End)]
    [TestCase(Alignment.Start)]
    public void Flex1d_Line_NormalSettingOut_StretcherHeader(Alignment settingOut)
    {
        // Arrange
        Flex1d builder = new Flex1d()
            .Bounds(Line, CrossAxis)
            .MainJustification(Justification.SpaceEvenly)
            .CrossAlignment(Alignment.Start)
            .CrossSettingOut(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(settingOut)
            .Pattern(StretcherHeader);
        ExecuteTest(builder);
    }

    [TestCase(Alignment.Center)]
    [TestCase(Alignment.End)]
    [TestCase(Alignment.Start)]
    public void Flex1d_Line_CrossSettingOut_StretcherHeader(Alignment settingOut)
    {
        // Arrange
        Flex1d builder = new Flex1d()
            .Bounds(Line, CrossAxis)
            .MainJustification(Justification.SpaceEvenly)
            .CrossAlignment(Alignment.Start)
            .CrossSettingOut(settingOut)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .Pattern(StretcherHeader);
        ExecuteTest(builder);
    }

    [TestCase(Alignment.Center)]
    [TestCase(Alignment.End)]
    [TestCase(Alignment.Start)]
    public void Flex1d_Line_NormalAlignment(Alignment alignment)
    {
        // Arrange
        Flex1d builder = new Flex1d()
            .Bounds(Line, CrossAxis)
            .MainJustification(Justification.SpaceEvenly)
            .CrossAlignment(Alignment.Start)
            .CrossSettingOut(Alignment.Start)
            .NormalAlignment(alignment)
            .NormalSettingOut(Alignment.Start)
            .Pattern(StretcherSoldier);
        ExecuteTest(builder);
    }

    [TestCase(Justification.Center, Alignment.Center)]
    [TestCase(Justification.Start, Alignment.Start)]
    [TestCase(Justification.End, Alignment.End)]
    public void Flex1d_Vertical(Justification justification, Alignment alignment)
    {
        // Arrange
        Vector3 start = new(-1, -1, -1);
        Vector3 end = new(-1, -1, 3);
        Line line = new(start, end);
        Flex1d builder = new Flex1d()
            .Bounds(line, Vector3.XAxis)
            .MainJustification(justification)
            .CrossAlignment(alignment)
            .CrossSettingOut(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .Pattern(StretcherSoldier);

        // Act
        //IReadOnlyCollection<ElementInstance> instances = builder.ToInstances().ToArray();
        //ElementInstance assembly = builder.ToAssembly();
        IReadOnlyCollection<ElementInstance> components = builder.ToComponents();

        // Preview
        //Model.AddBounds(instances, MaterialByName("Aqua"));
        //Model.AddBounds(instances.Select(x => x.BaseDefinition), MaterialByName("Orange"));
        //Model.AddBounds(assembly, MaterialByName("Aqua"));
        //Model.AddBounds(assembly.BaseDefinition, MaterialByName("Orange"));
        Model.AddElements(components);
        Model.AddBounds(components);
        Model.AddElements(CreateModelArrows.ByLine(line, Colors.Black));

        // Assert
        Verify.ElementsByBounds(components);
    }

    [TestCase(Alignment.Center)]
    [TestCase(Alignment.Start)]
    [TestCase(Alignment.End)]
    public void Flex1d_InvertedCrossAxis_CrossAlignment_StretcherHeader(Alignment alignment)
    {
        // Arrange
        Flex1d builder = new Flex1d()
            .Bounds(Vector3.XAxis, Vector3.YAxis.Negate(), Vector3.ZAxis, 4)
            .MainJustification(Justification.SpaceBetween)
            .CrossAlignment(alignment)
            .NormalAlignment(Alignment.Center)
            .CrossSettingOut(Alignment.Center)
            .NormalSettingOut(Alignment.Center)
            .Pattern(StretcherHeader);

        // Act
        //IReadOnlyCollection<ElementInstance> instances = builder.ToInstances().ToArray();
        //ElementInstance assembly = builder.ToAssembly();
        IReadOnlyCollection<ElementInstance> components = builder.ToComponents();

        // Preview
        //Model.AddBounds(instances, MaterialByName("Aqua"));
        //Model.AddBounds(instances.Select(x => x.BaseDefinition), MaterialByName("Orange"));
        //Model.AddBounds(assembly, MaterialByName("Aqua"));
        //Model.AddBounds(assembly.BaseDefinition, MaterialByName("Orange"));
        Model.AddElements(builder._curve);
        Model.AddElements(components);
        Model.AddBounds(components);

        // Assert
        Verify.ElementsByBounds(components);
    }

    [TestCase(Justification.Center)]
    [TestCase(Justification.End)]
    [TestCase(Justification.SpaceAround)]
    [TestCase(Justification.SpaceBetween)]
    [TestCase(Justification.SpaceEvenly)]
    [TestCase(Justification.Start)]
    public void Flex1d_Arc_Justification(Justification justification)
    {
        // Arrange
        Flex1d builder = new Flex1d()
            .Bounds(Arc)
            .MainJustification(justification)
            .CrossAlignment(Alignment.Start)
            .CrossSettingOut(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .Pattern(StretcherHeader);

        // Act
        IReadOnlyCollection<ElementInstance> components = builder.ToComponents();

        // Preview
        Model.AddElements(components);
        Model.AddBounds(components);
        Model.AddElements(new ModelCurve(Arc, MaterialByName("Black")));

        // Assert
        Verify.ElementsByBounds(components);
    }

    private void ExecuteTest(Flex1d builder)
    {
        // Act
        IReadOnlyCollection<ElementInstance> components = builder.ToComponents();

        // Preview
        Model.AddElements(components);
        Model.AddBounds(components);
        Model.AddElements(CreateModelArrows.ByLine(Line, Colors.Black));

        // Assert
        Verify.ElementsByBounds(components);
    }
}
