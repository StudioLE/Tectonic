using Lineweights.Flex.Coordination;
using Lineweights.Flex;
using Lineweights.Flex.Sequences;

namespace Lineweights.Drawings;

/// <summary>
/// Build a <see cref="Sheet"/> using a <see href="https://refactoring.guru/design-patterns/builder">builder pattern</see>.
/// </summary>
public sealed class SheetBuilder
{
    #region Sheet

    private double _sheetWidth;
    private double _sheetHeight;
    private Spacing? _sheetPadding;
    private Spacing? _sheetMargin;

    /// <summary>
    /// Set the <see cref="Sheet"/> width and height.
    /// </summary>
    public SheetBuilder SheetSize(double width, double height)
    {
        _sheetWidth = width;
        _sheetHeight = height;
        return this;
    }

    /// <summary>
    /// Set the <see cref="Sheet"/> padding.
    /// </summary>
    public SheetBuilder SheetPadding(double width, double height)
    {
        _sheetPadding = new(width, height, 0);
        return this;
    }

    /// <summary>
    /// Set the <see cref="Sheet"/> margin.
    /// </summary>
    public SheetBuilder SheetMargin(double width, double height)
    {
        _sheetMargin = new(width, height, 0);
        return this;
    }

    #endregion

    #region Title

    private double? _titleWidth;
    private double? _titleHeight;
    private Spacing? _titlePadding;
    private Spacing? _titleMargin;

    /// <summary>
    /// Set the height of a horizontally oriented <see cref="Sheet.Title"/>.
    /// </summary>
    public SheetBuilder HorizontalTitleArea(double height)
    {
        _titleHeight = height;
        return this;
    }

    /// <summary>
    /// Set the width of a vertically oriented <see cref="Sheet.Title"/>.
    /// </summary>
    public SheetBuilder VerticalTitleArea(double width)
    {
        _titleWidth = width;
        return this;
    }

    /// <summary>
    /// Set the <see cref="Sheet.Title"/> padding.
    /// </summary>
    public SheetBuilder TitlePadding(double width, double height)
    {
        _titlePadding = new(width, height, 0);
        return this;
    }

    /// <summary>
    /// Set the <see cref="Sheet.Title"/> margin.
    /// </summary>
    public SheetBuilder TitleMargin(double width, double height)
    {
        _titleMargin = new(width, height, 0);
        return this;
    }

    #endregion

    #region Content

    private IReadOnlyCollection<View>? _views;
    private Spacing? _contentPadding;
    private Spacing? _contentMargin;
    private Flex2d _viewArrangement = new Flex2d()
        .Orientation(Vector3.XAxis, Vector3.YAxis.Negate(), Vector3.ZAxis)
        .MainJustification(Justification.SpaceEvenly)
        .CrossJustification(Justification.SpaceEvenly);
    //.CrossAlignment(Alignment.Start);

    /// <summary>
    /// Set the <see cref="Sheet.Content"/> padding.
    /// </summary>
    public SheetBuilder ContentPadding(double width, double height)
    {
        _contentPadding = new(width, height, 0);
        return this;
    }

    /// <summary>
    /// Set the <see cref="Sheet.Content"/> margin.
    /// </summary>
    public SheetBuilder ContentMargin(double width, double height)
    {
        _contentMargin = new(width, height, 0);
        return this;
    }

    /// <summary>
    /// Set the views to be placed on the sheet.
    /// The position of the view is then set by the <see cref="ViewArrangement(Flex2d)"/>.
    /// </summary>
    public SheetBuilder Views(IReadOnlyCollection<View> views)
    {
        _views = views;
        return this;
    }

    /// <summary>
    /// Set the arrangement of the views as a <see cref="Flex2d"/>.
    /// This method overrides the existing or default ViewArrangement.
    /// The <see cref="Flex2d"/> is built with the <see cref="Views(IReadOnlyCollection{View})"/> as its elements.
    /// </summary>
    public SheetBuilder ViewArrangement(Flex2d viewArrangement)
    {
        _viewArrangement = viewArrangement;
        return this;
    }

    /// <summary>
    /// Set the arrangement of the views as a <see cref="Flex2d"/>.
    /// This method extends the existing or default ViewArrangement.
    /// The <see cref="Flex2d"/> is built with the <see cref="Views(IReadOnlyCollection{View})"/> as its elements.
    /// </summary>
    public SheetBuilder ViewArrangement(Action<Flex2d> viewArrangement)
    {
        viewArrangement.Invoke(_viewArrangement);
        return this;
    }

    #endregion;

    #region Execution methods

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
        SequenceBuilder sequence = WrappingSequence.WithoutOverflow(instances);
        _viewArrangement
            .Container(content)
            .MainPatterns(sequence);

        IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> components = _viewArrangement.ToComponents();

        // TODO: Rather than ToComponents can we just Build, inspect the proxy and then manually apply the Proxy translation?
        
        content.Views = components.SelectMany(x => x).ToArray();

        return content;
    }

    #endregion
}
