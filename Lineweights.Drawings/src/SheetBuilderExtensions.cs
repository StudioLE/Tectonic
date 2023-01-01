namespace Lineweights.Drawings;

/// <summary>
/// Build methods for <see cref="SheetBuilder"/>
/// </summary>
public static class SheetBuilderExtensions
{
    #region Sheet

    /// <summary>
    /// Set the <see cref="Sheet"/> width and height.
    /// </summary>
    public static SheetBuilder SheetSize(this SheetBuilder @this, double width, double height)
    {
        @this._sheetWidth = width;
        @this._sheetHeight = height;
        return @this;
    }

    /// <summary>
    /// Set the <see cref="Sheet"/> padding.
    /// </summary>
    public static SheetBuilder SheetPadding(this SheetBuilder @this, double width, double height)
    {
        @this._sheetPadding = new(width, height, 0);
        return @this;
    }

    /// <summary>
    /// Set the <see cref="Sheet"/> margin.
    /// </summary>
    public static SheetBuilder SheetMargin(this SheetBuilder @this, double width, double height)
    {
        @this._sheetMargin = new(width, height, 0);
        return @this;
    }

    #endregion

    #region Title

    /// <summary>
    /// Set the height of a horizontally oriented <see cref="Sheet.Title"/>.
    /// </summary>
    public static SheetBuilder HorizontalTitleArea(this SheetBuilder @this, double height)
    {
        @this._titleHeight = height;
        return @this;
    }

    /// <summary>
    /// Set the width of a vertically oriented <see cref="Sheet.Title"/>.
    /// </summary>
    public static SheetBuilder VerticalTitleArea(this SheetBuilder @this, double width)
    {
        @this._titleWidth = width;
        return @this;
    }

    /// <summary>
    /// Set the <see cref="Sheet.Title"/> padding.
    /// </summary>
    public static SheetBuilder TitlePadding(this SheetBuilder @this, double width, double height)
    {
        @this._titlePadding = new(width, height, 0);
        return @this;
    }

    /// <summary>
    /// Set the <see cref="Sheet.Title"/> margin.
    /// </summary>
    public static SheetBuilder TitleMargin(this SheetBuilder @this, double width, double height)
    {
        @this._titleMargin = new(width, height, 0);
        return @this;
    }

    #endregion

    #region Content

    /// <summary>
    /// Set the <see cref="Sheet.Content"/> padding.
    /// </summary>
    public static SheetBuilder ContentPadding(this SheetBuilder @this, double width, double height)
    {
        // TODO: Implement as an interface...
        @this._contentPadding = new(width, height, 0);
        return @this;
    }

    /// <summary>
    /// Set the <see cref="Sheet.Content"/> margin.
    /// </summary>
    public static SheetBuilder ContentMargin(this SheetBuilder @this, double width, double height)
    {
        @this._contentMargin = new(width, height, 0);
        return @this;
    }

    /// <summary>
    /// Set the views to be placed on the sheet.
    /// The position of the view is then set by the <see cref="ViewArrangement(SheetBuilder, IDistribution2dBuilder)"/>.
    /// </summary>
    public static SheetBuilder Views(this SheetBuilder @this, IReadOnlyCollection<View> views)
    {
        @this._views = views;
        return @this;
    }

    /// <summary>
    /// Set the arrangement of the views as a <see cref="IDistribution2dBuilder"/>.
    /// This method overrides the existing or default ViewArrangement.
    /// The <see cref="IDistribution2dBuilder"/> is built with the <see cref="Views(SheetBuilder, IReadOnlyCollection{View})"/> as its elements.
    /// </summary>
    public static SheetBuilder ViewArrangement(this SheetBuilder @this, IDistribution2dBuilder viewArrangement)
    {
        @this._viewArrangement = viewArrangement;
        return @this;
    }

    /// <summary>
    /// Set the arrangement of the views as a <see cref="IDistribution2dBuilder"/>.
    /// This method extends the existing or default ViewArrangement.
    /// The <see cref="IDistribution2dBuilder"/> is built with the <see cref="Views(SheetBuilder, IReadOnlyCollection{View})"/> as its elements.
    /// </summary>
    public static SheetBuilder ViewArrangement(this SheetBuilder @this, Action<IDistribution2dBuilder> viewArrangement)
    {
        viewArrangement.Invoke(@this._viewArrangement);
        return @this;
    }

    #endregion;
}
