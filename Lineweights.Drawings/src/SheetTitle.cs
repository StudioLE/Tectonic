using StudioLE.Core.Results;

namespace Lineweights.Drawings;

/// <summary>
/// A text field within a <see cref="SheetTitle"/>.
/// </summary>
public struct TextField
{
    /// <summary>
    /// The index used to order <see cref="TextField"/> in the <see cref="SheetTitle"/>.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// The title used as a key.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The content.
    /// </summary>
    public string Content { get; set; }
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
    public IResult<TextField> TryGetField(string title)
    {
        return _fields.TryGetValue(title, out TextField field)
            ? new Success<TextField>(field)
            : new Failure<TextField>("Not found.");
    }

    /// <summary>
    /// Update the content of an existing <see cref="TextField"/>.
    /// </summary>
    public IResult TryUpdateField(string title, string content)
    {
        IResult<TextField> result = TryGetField(title);
        if (result is not Success<TextField> success)
            return new Failure(result.Errors);
        TextField field = success.Value with
        {
            Content = content
        };
        _fields[title] = field;
        return new Success();
    }

    /// <summary>
    /// Add a <see cref="TextField"/>.
    /// The field is placed at the end of the collection.
    /// </summary>
    public IResult<TextField> TryAddField(string title, string content)
    {
        IResult<TextField> result = TryGetField(title);
        if (result is Success<TextField>)
            return new Failure<TextField>("A field with that title already exists.");
        TextField field = new()
        {
            Order = _fields.Count,
            Title = title,
            Content = content
        };
        _fields[title] = field;
        return new Success<TextField>(field);
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
