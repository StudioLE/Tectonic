namespace Lineweights.Drawings;

/// <summary>
/// A drawing sheet that contains a <see cref="SheetTitle"/> and a <see cref="SheetContent"/>
/// positioned on its <see cref="Canvas"/>.
/// <inheritdoc cref="Canvas"/>
/// <see cref="Sheet"/> are constructed by the <see cref="SheetBuilder"/>.
/// </summary>
public sealed class Sheet : Canvas
{
    /// <inheritdoc cref="SheetTitle"/>
    public SheetTitle Title { get; set; } = new();

    /// <inheritdoc cref="SheetContent"/>
    public SheetContent Content { get; set; } = new();

    /// <inheritdoc cref="Sheet"/>
    internal Sheet()
    {
    }

    /// <summary>
    /// Render the sheet border, the <see cref="SheetTitle"/> and the <see cref="SheetContent"/>.
    /// Each of the <see cref="SheetContent.Views"/> are rendered by their respective
    /// <see cref="View.Render()"/> methods.
    /// </summary>
    public IEnumerable<GeometricElement> Render()
    {
        IEnumerable<GeometricElement> views = Content
            .Views
            .SelectMany(instance =>
            {
                View view = Validate.IsTypeOrThrow<View>(instance.BaseDefinition, "Failed to render the sheet.");
                return view
                    .Render()
                    .Select(x =>
                    {
                        x.Transform.Concatenate(instance.Transform);
                        x.Transform.Concatenate(Content.Transform);
                        return x;
                    });
            });

        ModelCurve sheetOutline = new(Border);
        ModelCurve contentOutline = new(Content.Border, null, Content.Transform);
        ModelCurve titleOutline = new(Title.Border, null, Title.Transform);

        // TODO: Render SheetTitle

        return Enumerable.Empty<GeometricElement>()
            .Append(sheetOutline)
            .Append(contentOutline)
            .Append(titleOutline)
            .Concat(views);
    }
}
