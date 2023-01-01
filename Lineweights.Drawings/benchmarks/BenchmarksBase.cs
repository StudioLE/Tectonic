using Lineweights.Flex;
using Lineweights.Workflows.Samples;

namespace Lineweights.Drawings.Benchmarks;

public abstract class BenchmarksBase
{
    protected readonly IReadOnlyCollection<Element> _brickwork;
    protected readonly View _view;
    protected readonly Sheet _sheet;

    protected BenchmarksBase()
    {
        _brickwork = Scenes.Brickwork();

        _view = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .ViewDirection(ViewDirection.Front)
            .ElementsInView(_brickwork)
            .Build();

        ViewDirection[] viewDirections =
        {
            ViewDirection.Top,
            ViewDirection.Bottom,
            ViewDirection.Left,
            ViewDirection.Right,
            ViewDirection.Front,
            ViewDirection.Back
        };
        ViewBuilder viewBuilder = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .Scale(1d / 10)
            .ElementsInView(_brickwork);
        View[] views = viewDirections
            .Select(direction => viewBuilder
                .ViewDirection(direction)
                .Build())
            .ToArray();

        ISequenceBuilder sequenceBuilder = new SequenceBuilder();
        IDistribution2dBuilder defaultViewArrangement = new DefaultViewArrangement();
        IBuilder<Sheet> builder = new SheetBuilder(sequenceBuilder, defaultViewArrangement)
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views);
        _sheet = builder.Build();
    }
}
