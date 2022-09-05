using Lineweights.Flex.Sequences;
using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Flex.Tests;

[VisualizeAfterTest]
internal sealed class Flex2dTests
{
    private readonly Model _model = new();
    private Brick Container { get; }
    private SequenceBuilder StretcherSoldier { get; }
    private SequenceBuilder SoldierStretcher { get; }
    private SequenceBuilder StretcherHeader { get; }

    public Flex2dTests()
    {
        const double width = 4;
        const double length = 1;
        const double height = 2;
        const double spacing = 0;
        Container = new(width, length, height, spacing, "Container")
        {
            Material = MaterialByName("Gray")
        };
        StretcherSoldier = RepeatingSequence.MaxCount(20, Brick.Stretcher.CreateInstance(), Brick.Soldier.CreateInstance());
        SoldierStretcher = RepeatingSequence.MaxCount(20, Brick.Soldier.CreateInstance(), Brick.Stretcher.CreateInstance());
        StretcherHeader = RepeatingSequence.MaxCount(20, Brick.Stretcher.CreateInstance(), Brick.Header.CreateInstance());
    }

    [SetUp]
    public void Setup()
    {
        _model.AddBounds(Container, Container.Material);
        _model.AddElements(CreateModelArrows.ByTransform(new()));
    }

    [TestCase(Justification.Center)]
    [TestCase(Justification.End)]
    [TestCase(Justification.SpaceAround)]
    [TestCase(Justification.SpaceBetween)]
    [TestCase(Justification.SpaceEvenly)]
    [TestCase(Justification.Start)]
    public async Task Flex2d_MainJustification(Justification justification)
    {
        // Arrange
        Flex2d builder = new Flex2d()
            .Container(Container)
            .MainJustification(justification)
            .CrossAlignment(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .MainPatterns(StretcherSoldier, SoldierStretcher);
        await ExecuteTest(builder);
    }

    [TestCase(Alignment.Center)]
    [TestCase(Alignment.End)]
    [TestCase(Alignment.Start)]
    public async Task Flex2d_CrossAlignment(Alignment crossAlignment)
    {
        // Arrange
        Flex2d builder = new Flex2d()
            .Container(Container)
            .MainJustification(Justification.Start)
            .CrossAlignment(crossAlignment)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .MainPatterns(StretcherHeader, StretcherHeader);
        await ExecuteTest(builder);
    }

    [TestCase(Justification.Center)]
    [TestCase(Justification.End)]
    [TestCase(Justification.SpaceAround)]
    [TestCase(Justification.SpaceBetween)]
    [TestCase(Justification.SpaceEvenly)]
    [TestCase(Justification.Start)]
    public async Task Flex2d_CrossJustification(Justification crossJustification)
    {
        // Arrange
        Flex2d builder = new Flex2d()
            .Container(Container)
            .MainJustification(Justification.Start)
            .CrossJustification(crossJustification)
            .CrossAlignment(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .MainPatterns(StretcherSoldier, SoldierStretcher);
        await ExecuteTest(builder);
    }

    [TestCase(Alignment.Center)]
    [TestCase(Alignment.End)]
    [TestCase(Alignment.Start)]
    public async Task Flex2d_NormalSettingOut(Alignment alignment)
    {
        // Arrange
        Flex2d builder = new Flex2d()
            .Container(Container)
            .MainJustification(Justification.Start)
            .CrossJustification(Justification.Start)
            .CrossAlignment(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(alignment)
            .MainPatterns(StretcherSoldier, SoldierStretcher);
        await ExecuteTest(builder);
    }

    [TestCase(Justification.Center)]
    [TestCase(Justification.End)]
    [TestCase(Justification.SpaceAround)]
    [TestCase(Justification.SpaceBetween)]
    [TestCase(Justification.SpaceEvenly)]
    [TestCase(Justification.Start)]
    public async Task Flex2d_Vertical_CrossJustification(Justification crossJustification)
    {
        // Arrange
        Flex2d builder = new Flex2d()
            .Container(Container)
            .Orientation(Vector3.XAxis, Vector3.ZAxis, Vector3.YAxis)
            .MainJustification(Justification.Start)
            .CrossJustification(crossJustification)
            .CrossAlignment(Alignment.Center)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .MainPatterns(StretcherSoldier, SoldierStretcher);
        await ExecuteTest(builder);
    }

    [TestCase(Justification.Center)]
    [TestCase(Justification.End)]
    [TestCase(Justification.SpaceAround)]
    [TestCase(Justification.SpaceBetween)]
    [TestCase(Justification.SpaceEvenly)]
    [TestCase(Justification.Start)]

    public async Task Flex2d_InvertedCrossAxis(Justification justification)
    {
        // Arrange
        Flex2d builder = new Flex2d()
            .Container(Container)
            .Orientation(Vector3.XAxis, Vector3.YAxis.Negate(), Vector3.ZAxis)
            .MainJustification(Justification.Start)
            .CrossJustification(justification)
            .CrossAlignment(Alignment.Center)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .MainPatterns(StretcherHeader)
            .CrossPattern(RepeatingSequence.MaxCount(3));
        await ExecuteTest(builder);
    }

    private async Task ExecuteTest(Flex2d builder)
    {
        // Act
        IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> components = builder.Build();

        // Preview
        foreach (ElementInstance instance in builder.Assemblies)
        {
            _model.AddBounds(instance, MaterialByName("Aqua"));
            _model.AddBounds(instance.BaseDefinition, MaterialByName("Orange"));
        }
        _model.AddElements(components.SelectMany(x => x));
        _model.AddBounds(components.SelectMany(x => x));

        // Assert
        await Verify.ElementsByBounds(components.SelectMany(x => x).ToArray());
    }
}
