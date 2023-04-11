namespace Geometrician.Core.Constraints;

/// <summary>
/// A constraint to validate that an index exists on a collection.
/// </summary>
public sealed class IndexConstraint : Constraint<int?>
{
    private readonly int _collectionCount;
    private readonly string _indexName;

    /// <inheritdoc/>
    public IndexConstraint(int value, int collectionCount, string indexName, string? paramName) : base(value, paramName)
    {
        _collectionCount = collectionCount;
        _indexName = indexName;
    }

    /// <inheritdoc/>
    public override bool IsValid()
    {
        return Value >= 0 || Value < _collectionCount;
    }

    /// <inheritdoc/>
    public override string Message()
    {
        string message = $"The {ParamName ?? "specified"} {_indexName} index";
        message += Value is null
            ? string.Empty
            : $" ({Value})";
        message += " does not exist.";
        return message;
    }

    /// <inheritdoc/>
    public override Exception Exception()
    {
        return new ArgumentException(Message(), ParamName);
    }
}
