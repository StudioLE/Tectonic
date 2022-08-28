using Lineweights.Workflows.Containers;

namespace Lineweights.Workflows.Results;

/// <summary>
/// A strategy to visualise or sample results of an execution.
/// </summary>
public interface IResultStrategy
{
    /// <inheritdoc cref="IResultStrategy"/>
    public Task<Container> Execute(Model model, DocumentInformation doc);
}
