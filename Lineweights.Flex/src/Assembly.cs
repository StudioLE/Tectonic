namespace Lineweights.Flex;

/// <summary>
/// An assembly of <see cref="ElementInstance"/> components.
/// </summary>
internal sealed class Assembly : GeometricElement, IHasBounds
{
    /// <inheritdoc/>
    public BBox3 Bounds { get; }

    /// <summary>
    /// The components in the assembly.
    /// </summary>
    public IReadOnlyCollection<ElementInstance> Components { get; init; }

    /// <inheritdoc cref="Assembly"/>
    public Assembly(IReadOnlyCollection<ElementInstance> components)
    {
        IsElementDefinition = true;
        Components = components;
        // TODO: Note we have changed behaviour. The instances are not normalised to origin.
        Bounds = CreateBBox3.ByElements(components);
    }

    /// <inheritdoc cref="Assembly"/>
    public Assembly(IReadOnlyCollection<ElementInstance> components, BBox3 bounds)
    {
        IsElementDefinition = true;
        Components = components;
        // TODO: Note we have changed behaviour. The instances are not normalised to origin.
        Bounds = bounds;
    }
}

/// <summary>
/// Methods to extend <see cref="Assembly"/>.
/// </summary>
public static class AssemblyExtensions
{
    // TODO: Move this to the Assembly object and accept the instance transform as a parameter.
    /// <summary>
    /// Get the components transformed from an <see cref="ElementInstance"/> of an <see cref="Assembly"/>.
    /// </summary>
    public static IReadOnlyCollection<ElementInstance> ToComponents(this ElementInstance assemblyInstance)
    {
        Assembly assembly = Validate.IsTypeOrThrow<Assembly>(assemblyInstance.BaseDefinition, "Failed to get components from assembly.");
        return assembly
            .Components
            .Select(component =>
            {
                Transform transform = component
                    .Transform
                    .Concatenated(assembly.Transform)
                    .Concatenated(assemblyInstance.Transform);
                return component
                    .BaseDefinition
                    .CreateInstance(transform, component.Name);
            })
            .ToArray();
    }
}
