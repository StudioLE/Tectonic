namespace Lineweights.Core.Abstractions;

public interface IDistribution2dBuilder
{
    /// <summary>
    /// Set the main sequence.
    /// </summary>
    public IDistribution2dBuilder MainSequence(params ISequenceBuilder[] sequences);

    /// <summary>
    /// Set the container to distribute inside.
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    public IDistribution2dBuilder Container(GeometricElement container);

    /// <summary>
    /// Distribute the elements.
    /// </summary>
    public IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> Build();
}
