using Ardalis.Result;

namespace Lineweights.Drawings;

/// <summary>
/// A text field within a <see cref="SheetTitle"/>.
/// </summary>
public struct TextField
{
    /// <summary>
    /// The index used to order <see cref="TextField"/> in the <see cref="SheetTitle"/>.
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// The title used as a key.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// The content.
    /// </summary>
    public string Content { get; init; }
}

/// <summary>
/// The title space or title block of a <see cref="Sheet"/> which contains the <see cref="TextField"/>.
/// </summary>
public sealed class SheetTitle : Canvas
{
    private readonly Dictionary<string, TextField> _fields = new();

    /// <inheritdoc cref="SheetTitle"/>
    internal SheetTitle()
    {
    }

    /// <summary>
    /// Get a <see cref="TextField"/> by its title.
    /// </summary>
    public Result<TextField> TryGetField(string title)
    {
        return _fields.TryGetValue(title, out TextField field)
            ? field
            : Result<TextField>.NotFound();
    }

    /// <summary>
    /// Update the content of an existing <see cref="TextField"/>.
    /// </summary>
    public Result<TextField> TryUpdateField(string title, string content)
    {
        Result<TextField> result = TryGetField(title);
        if (!result.IsSuccess)
            return result;
        TextField field = new()
        {
            Order = result.Value.Order,
            Title = result.Value.Title,
            Content = content
        };
        _fields[title] = field;
        return field;
    }

    /// <summary>
    /// Add a <see cref="TextField"/>.
    /// The field is placed at the end of the collection.
    /// </summary>
    public Result<TextField> TryAddField(string title, string content)
    {
        Result<TextField> result = TryGetField(title);
        if (result.IsSuccess)
            return Result<TextField>.Error("A field with that title already exists.");
        TextField field = new()
        {
            Order = _fields.Count,
            Title = title,
            Content = content
        };
        _fields[title] = field;
        return field;
    }

    /// <summary>
    /// Get all the <see cref="TextField"/> in order.
    /// </summary>
    /// <returns></returns>
    public IOrderedEnumerable<TextField> AllFieldsInOrder()
    {
        return _fields
            .Values
            .OrderBy(x => x.Order);
    }
}
