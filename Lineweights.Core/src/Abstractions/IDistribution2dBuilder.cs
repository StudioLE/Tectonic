namespace Lineweights.Core.Abstractions;

public interface IDistribution2dBuilder
{
    public IReadOnlyCollection<ISequenceBuilder> MainSequences { set; }

    /// <summary>
    /// Set the container to distribute inside.
    /// </summary>
    public GeometricElement Container { set; }

    /// <summary>
    /// Distribute the elements.
    /// </summary>
    public IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> Build();
}
