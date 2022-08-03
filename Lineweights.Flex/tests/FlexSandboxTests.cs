#if false
using Lineweights.Diagnostics;
using Lineweights.Core.Elements;
using Lineweights.Flex.Coordination;
using Lineweights.Flex.Elements;
using Lineweights.Flex;
using Lineweights.Flex.Patterns;

namespace Lineweights.Flex.Tests.Flex;

internal sealed class FlexSandboxTests : Sample
{
    private Brick Container { get; }
    private PatternBuilder StretcherSoldier { get; }
    private PatternBuilder SoldierStretcher { get; }
    private PatternBuilder StretcherHeader { get; }

    public FlexSandboxTests()
    {
        const double width = 4;
        const double length = 1;
        const double height = 2;
        const double spacing = 0;
        Container = new(width, length, height, spacing, "Container")
        {
            Material = MaterialByName("Gray")
        };
        StretcherSoldier = RepeatingPattern.MaxCount(20, Brick.Stretcher.CreateInstance(), Brick.Soldier.CreateInstance());
        SoldierStretcher = RepeatingPattern.MaxCount(20, Brick.Soldier.CreateInstance(), Brick.Stretcher.CreateInstance());
        StretcherHeader = RepeatingPattern.MaxCount(20, Brick.Stretcher.CreateInstance(), Brick.Header.CreateInstance());
    }

    [SetUp]
    public void Setup()
    {
        Model.AddBounds(Container, Container.Material);
        Model.AddElements(CreateModelArrows.ByTransform(new()));
    }

    [TestCase(Justification.Center)]
    [TestCase(Justification.End)]
    [TestCase(Justification.SpaceAround)]
    [TestCase(Justification.SpaceBetween)]
    [TestCase(Justification.SpaceEvenly)]
    [TestCase(Justification.Start)]
    [Explicit("Sandbox")]
    public void Flex2d_Sandbox(Justification justification)
    {
        // ReSharper disable InconsistentNaming
        Brick _container = Container;
        Flex1d _builder;
        Vector3 _settingOutUVW = new(0, 0, 0);
        Vector3 _mainAxis = Vector3.XAxis;
        Vector3 _crossAxis = Vector3.YAxis;
        Vector3 _normalAxis = Vector3.ZAxis;
        Justification _mainJustification = justification;
        Alignment _crossAlignment = Alignment.Center;
        Alignment _normalAlignment = Alignment.Center;
        PatternBuilder[] _mainPatterns = { StretcherSoldier, SoldierStretcher };
        PatternBuilder _crossPattern = RepeatingPattern.MaxCount(2);
        Justification _crossJustification = justification;
        // ReSharper restore InconsistentNaming

        if (_container is null)
            throw new ArgumentException("Container must be set before Build() is called.", nameof(_container));

        Vector3 settingOutPoint = _container.Bounds.PointAt(_settingOutUVW);

        // Distribute in the main axis
        _builder = new Flex1d()
            .Bounds(_mainAxis, _crossAxis, _normalAxis, _mainAxis.Dimension(_container.Bounds))
            .MainJustification(_mainJustification)
            .CrossSettingOut(Alignment.Center)
            .NormalSettingOut(Alignment.Center)
            .CrossAlignment(_crossAlignment)
            .NormalAlignment(_normalAlignment);

        double normalComponent = _normalAxis.Dimension(_container.Bounds());

        IReadOnlyCollection<ElementInstance> assemblies1d = _mainPatterns
            .SelectMany(pattern => pattern is WrappingPattern wrapping
                ? wrapping.Split(_builder)
                : new[] { pattern })
            .Select(pattern1d => _builder
                .Pattern(pattern1d)
                .ToAssembly(normalComponent))
            //.Select(instance =>
            //{
            //    if (instance.BaseDefinition is not Assembly assembly)
            //        throw new("Expected an assembly.");

            //    // TODO: THis should occur on .TOAssembly();

            //    Vector3 center = assembly.Min.Average(assembly.Max);
            //    double normalComponent = _normalAxis.Dimension(_container.Bounds());
            //    BBox3 bounds = new(new[]{
            //        center + _normalAxis * normalComponent * 0.5,
            //        center + _normalAxis * normalComponent * 0.5 * -1,
            //        assembly.Min,
            //        assembly.Max
            //    });
            //    assembly.Min = bounds.Min;
            //    assembly.Max = bounds.Max;

            //        return instance;
            //})
            .ToArray();



        _crossPattern.Items(assemblies1d.ToArray());

        double check = _crossAxis.Dimension(_container.Bounds);
        Model.AddBounds(assemblies1d, MaterialByName("Aqua"));
        Model.AddBounds(assemblies1d.Select(x => x.BaseDefinition), MaterialByName("Orange"));

        // Distribute in the cross axis
        _builder = new Flex1d()
            .Bounds(_crossAxis, _mainAxis, _normalAxis, _crossAxis.Dimension(_container.Bounds))
            .MainJustification(_crossJustification)
            .CrossSettingOut(Alignment.Center)
            .NormalSettingOut(Alignment.Center)
            //.SetCrossAlignment(CrossAlignment)
            //.SetNormalAlignment(NormalAlignment)
            .Pattern(_crossPattern);

        //var assemblyInstances = _builder.ToInstances().ToArray();
        //Model.AddBounds(assemblyInstances, MaterialByName("Aqua"));
        //Model.AddBounds(assemblyInstances.Select(x => x.BaseDefinition), MaterialByName("Orange"));

        //ElementInstance assembly = _builder.ToAssembly();
        //Model.AddBounds(assembly, MaterialByName("Aqua"));
        //Model.AddBounds(assembly.BaseDefinition, MaterialByName("Orange"));

        IReadOnlyCollection<ElementInstance> components = _builder.ToComponents();
        Model.AddElements(components);
        Model.AddBounds(components);

    }
}
#endif
