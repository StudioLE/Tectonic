using Lineweights.Drawings;
using Lineweights.Flex;
using StudioLE.Core.Exceptions;

namespace Lineweights.Workflows.Samples;

internal static class SampleHelpers
{
    internal static View[] CreateViews(Model model, Scenes.Name sceneName)
    {
        double scopePadding = sceneName switch
        {
            Scenes.Name.Brickwork => .250,
            Scenes.Name.GeometricElements => .050,
            _ => throw new EnumSwitchException<Scenes.Name>("Failed to create views,", sceneName)
        };

        double scale = sceneName switch
        {
            Scenes.Name.Brickwork => 1d / 10,
            Scenes.Name.GeometricElements => 1d / 5,
            _ => throw new EnumSwitchException<Scenes.Name>("Failed to create views,", sceneName)
        };

        ViewBuilder viewBuilder = new ViewBuilder()
            .ScopePadding(scopePadding, scopePadding, scopePadding)
            .Scale(scale)
            .ElementsInView(model.Elements.Values.ToArray());

        ViewDirection[] viewDirections =
        {
            ViewDirection.Top,
            ViewDirection.Left,
            ViewDirection.Front
        };

        return viewDirections
            .Select(direction => viewBuilder
                .ViewDirection(direction)
                .Build())
            .ToArray();
    }

    internal static Sheet CreateSheet(Model model)
    {
        View[] views = model.AllElementsOfType<View>().ToArray();
        IBuilder<Sheet> builder = new SheetBuilder(new SequenceBuilder(), new DefaultViewArrangement())
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views);
        return builder.Build();
    }
}
