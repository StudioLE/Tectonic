using Lineweights.Workflows.Containers;

namespace Lineweights.Workflows.Results;

/// <summary>
/// A strategy to visualise or sample a <see cref="Model"/>.
/// </summary>
public interface IResultStrategy
{
    /// <inheritdoc cref="IResultStrategy"/>
    public Task<Container> Execute(Model model, DocumentInformation doc);
}
