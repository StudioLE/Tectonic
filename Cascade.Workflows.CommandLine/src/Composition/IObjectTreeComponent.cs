namespace Cascade.Workflows.CommandLine.Composition;

/// <summary>
/// A composite tree representation of an <see cref="object"/>.
/// </summary>
/// <remarks>
/// Follows a <see href="https://refactoring.guru/design-patterns/composite">composite design pattern</see>.
/// </remarks>
public interface IObjectTreeComponent
{
    /// <summary>
    /// The type of the <see cref="object"/>, or the underlying type if the type is nullable.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Is the type of the <see cref="object"/> nullable?
    /// </summary>
    public bool IsNullable { get; }

    /// <summary>
    /// The properties of the <see cref="object"/>.
    /// </summary>
    public IReadOnlyCollection<ObjectTreeProperty> Properties { get; }
}
