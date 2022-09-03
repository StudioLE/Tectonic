using Lineweights.Core.Documents;
using Lineweights.Workflows.Visualization;
using Microsoft.Extensions.Logging;

namespace Lineweights.Workflows.Execution;

/// <summary>
/// A workflow is an executable <see href="https://refactoring.guru/design-patterns/command">command</see>.
/// A workflow executes an activity
/// <see cref="OnResult"/> is called each time the workflow produces a result.
/// </summary>
public class WorkflowCommand
{
    private readonly ILogger _logger;
    private readonly ActivityCommand _activity;
    private readonly IVisualizationStrategy _visualizationStrategy;

    /// <inheritdoc cref="WorkflowCommand"/>
    public WorkflowCommand(ILogger logger, ActivityCommand activity, IVisualizationStrategy visualizationStrategy)
    {
        _logger = logger;
        _activity = activity;
        _visualizationStrategy = visualizationStrategy;
    }

    /// <summary>
    /// The method called each time a <see cref="Asset"/> is produced by the workflow.
    /// </summary>
    public void OnResult(object outputs)
    {
        if (((dynamic)outputs).Model is not Model model)
        {
            _logger.LogWarning("Activity output was not a model.");
            return;
        }
        DocumentInformation doc = new()
        {
            Name = _activity.Name,
            Description = $"Executed {_activity.Name}."
        };
        _visualizationStrategy.Execute(model, doc);
    }

    /// <summary>
    /// Execute the workflow command.
    /// </summary>
    public void Execute()
    {
        object outputs = _activity.Execute();
        OnResult(outputs);
    }
}
