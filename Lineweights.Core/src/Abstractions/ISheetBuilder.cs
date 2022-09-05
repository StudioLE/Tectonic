namespace Lineweights.Core.Abstractions;

/// <summary>
/// Build a sheet using a <see href="https://refactoring.guru/design-patterns/builder">builder pattern</see>.
/// </summary>
public interface ISheetBuilder
{
    /// <inheritdoc cref="ISheetBuilder"/>
    public object Build();
}
