using Lineweights.Core.Distribution;

namespace Lineweights.Drawings;

/// <summary>
/// Build a <see cref="Sheet"/> using a <see href="https://refactoring.guru/design-patterns/builder">builder pattern</see>.
/// </summary>
public sealed class SheetBuilder : IBuilder<Sheet>
{
    internal double _sheetWidth;
    internal double _sheetHeight;
    internal Spacing? _sheetPadding;
    internal Spacing? _sheetMargin;

    internal double? _titleWidth;
    internal double? _titleHeight;
    internal Spacing? _titlePadding;
    internal Spacing? _titleMargin;

    internal IReadOnlyCollection<View>? _views;
    internal Spacing? _contentPadding;
    internal Spacing? _contentMargin;

    private readonly ISequenceBuilder _sequenceBuilder;
    internal IDistribution2dBuilder _viewArrangement;
    // internal Flex2d _viewArrangement =

    public SheetBuilder(ISequenceBuilder sequenceBuilder, IDistribution2dBuilder viewArrangement)
    {
        _sequenceBuilder = sequenceBuilder;
        _viewArrangement = viewArrangement;
    }

    /// <summary>
    /// Build the <see cref="Sheet"/>.
    /// </summary>
    public Sheet Build()
    {
        BBox3 sheetBounds = CreateBBox3.ByLengths2d(_sheetWidth, _sheetHeight);

        if (_sheetWidth <= 0 || _sheetHeight <= 0)
            throw new("Failed to build sheet. Sheet width and height must be set.");

        if (_titleWidth is null && _titleHeight is null)
            throw new("Failed to build sheet. Title orientation must be set.");

        double titleWidth = _titleWidth ?? _sheetWidth;
        double titleHeight = _titleHeight ?? _sheetHeight;

        double contentWidth = _sheetWidth - (_titleWidth ?? 0);
        double contentHeight = _sheetHeight - (_titleHeight ?? 0);

        Transform titleTransform = new(_sheetWidth * 0.5 - titleWidth * 0.5, titleHeight * 0.5 - _sheetHeight * 0.5, 0);
        Transform contentTransform = new(contentWidth * 0.5 - _sheetWidth * 0.5, _sheetHeight * 0.5 - contentHeight * 0.5, 0);

        BBox3 titleBounds = CreateBBox3.ByLengths2d(titleWidth, titleHeight);
        BBox3 contentBounds = CreateBBox3.ByLengths2d(contentWidth, contentHeight);

        return new()
        {
            Bounds = sheetBounds,
            Margin = _sheetMargin ?? new(),
            Padding = _sheetPadding ?? new(),
            Title = BuildTitle(titleTransform, titleBounds),
            Content = BuildContent(contentTransform, contentBounds)
        };
    }

    /// <summary>
    /// Build the <see cref="SheetTitle"/>.
    /// </summary>
    private SheetTitle BuildTitle(Transform titleTransform, BBox3 titleBounds)
    {
        return new()
        {
            Bounds = titleBounds,
            Margin = _titleMargin ?? new(),
            Padding = _titlePadding ?? new(),
            Transform = titleTransform,
        };
    }

    /// <summary>
    /// Build the <see cref="SheetContent"/>.
    /// </summary>
    private SheetContent BuildContent(Transform contentTransform, BBox3 contentBounds)
    {
        SheetContent content = new()
        {
            Bounds = contentBounds,
            Margin = _contentMargin ?? new(),
            Padding = _contentPadding ?? new(),
            Transform = contentTransform,
        };

        if (_views is null)
            return content;

        // Arrange the views
        ElementInstance[] instances = _views
            .Select(x => x.CreateInstance())
            .ToArray();
        ISequenceBuilder sequence = _sequenceBuilder
            .Wrapping(true)
            .Body(instances);

        _viewArrangement.Container = content;
        _viewArrangement.MainSequences = new [] { sequence };

        IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> components = _viewArrangement.Build();

        // TODO: Rather than ToComponents can we just Build, inspect the proxy and then manually apply the Proxy translation?

        content.Views = components.SelectMany(x => x).ToArray();

        return content;
    }
}
