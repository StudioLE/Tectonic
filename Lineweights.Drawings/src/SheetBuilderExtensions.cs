using Lineweights.Flex;

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
    public static ISheetBuilder SheetSize(this ISheetBuilder @this, double width, double height)
    {
        ((SheetBuilder)@this)._sheetWidth = width;
        ((SheetBuilder)@this)._sheetHeight = height;
        return @this;
    }

    /// <summary>
    /// Set the <see cref="Sheet"/> padding.
    /// </summary>
    public static ISheetBuilder SheetPadding(this ISheetBuilder @this, double width, double height)
    {
        ((SheetBuilder)@this)._sheetPadding = new(width, height, 0);
        return @this;
    }

    /// <summary>
    /// Set the <see cref="Sheet"/> margin.
    /// </summary>
    public static ISheetBuilder SheetMargin(this ISheetBuilder @this, double width, double height)
    {
        ((SheetBuilder)@this)._sheetMargin = new(width, height, 0);
        return @this;
    }

    #endregion

    #region Title

    /// <summary>
    /// Set the height of a horizontally oriented <see cref="Sheet.Title"/>.
    /// </summary>
    public static ISheetBuilder HorizontalTitleArea(this ISheetBuilder @this, double height)
    {
        ((SheetBuilder)@this)._titleHeight = height;
        return @this;
    }

    /// <summary>
    /// Set the width of a vertically oriented <see cref="Sheet.Title"/>.
    /// </summary>
    public static ISheetBuilder VerticalTitleArea(this ISheetBuilder @this, double width)
    {
        ((SheetBuilder)@this)._titleWidth = width;
        return @this;
    }

    /// <summary>
    /// Set the <see cref="Sheet.Title"/> padding.
    /// </summary>
    public static ISheetBuilder TitlePadding(this ISheetBuilder @this, double width, double height)
    {
        ((SheetBuilder)@this)._titlePadding = new(width, height, 0);
        return @this;
    }

    /// <summary>
    /// Set the <see cref="Sheet.Title"/> margin.
    /// </summary>
    public static ISheetBuilder TitleMargin(this ISheetBuilder @this, double width, double height)
    {
        ((SheetBuilder)@this)._titleMargin = new(width, height, 0);
        return @this;
    }

    #endregion

    #region Content

    /// <summary>
    /// Set the <see cref="Sheet.Content"/> padding.
    /// </summary>
    public static ISheetBuilder ContentPadding(this ISheetBuilder @this, double width, double height)
    {
        ((SheetBuilder)@this)._contentPadding = new(width, height, 0);
        return @this;
    }

    /// <summary>
    /// Set the <see cref="Sheet.Content"/> margin.
    /// </summary>
    public static ISheetBuilder ContentMargin(this ISheetBuilder @this, double width, double height)
    {
        ((SheetBuilder)@this)._contentMargin = new(width, height, 0);
        return @this;
    }

    /// <summary>
    /// Set the views to be placed on the sheet.
    /// The position of the view is then set by the <see cref="ViewArrangement(ISheetBuilder, Flex2d)"/>.
    /// </summary>
    public static ISheetBuilder Views(this ISheetBuilder @this, IReadOnlyCollection<View> views)
    {
        ((SheetBuilder)@this)._views = views;
        return @this;
    }

    /// <summary>
    /// Set the arrangement of the views as a <see cref="Flex2d"/>.
    /// This method overrides the existing or default ViewArrangement.
    /// The <see cref="Flex2d"/> is built with the <see cref="Views(ISheetBuilder, IReadOnlyCollection{View})"/> as its elements.
    /// </summary>
    public static ISheetBuilder ViewArrangement(this ISheetBuilder @this, Flex2d viewArrangement)
    {
        ((SheetBuilder)@this)._viewArrangement = viewArrangement;
        return @this;
    }

    /// <summary>
    /// Set the arrangement of the views as a <see cref="Flex2d"/>.
    /// This method extends the existing or default ViewArrangement.
    /// The <see cref="Flex2d"/> is built with the <see cref="Views(ISheetBuilder, IReadOnlyCollection{View})"/> as its elements.
    /// </summary>
    public static ISheetBuilder ViewArrangement(this ISheetBuilder @this, Action<Flex2d> viewArrangement)
    {
        viewArrangement.Invoke(((SheetBuilder)@this)._viewArrangement);
        return @this;
    }

    #endregion;
}
