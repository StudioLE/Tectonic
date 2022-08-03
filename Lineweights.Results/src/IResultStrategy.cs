namespace Lineweights.Results;

/// <summary>
/// A strategy to visualise or sample results of an execution.
/// </summary>
public interface IResultStrategy
{
    /// <inheritdoc cref="IResultStrategy"/>
    public Result Execute(Model model, DocumentInformation metadata);
}
