using Geometrician.Core.Visualization;
using Lineweights.Core.Assets;
using Microsoft.Extensions.Logging;

namespace Geometrician.Core.Execution;

/// <summary>
/// A workflow is an executable <see href="https://refactoring.guru/design-patterns/command">command</see>.
/// A workflow executes an activity
/// <see cref="OnResult"/> is called each time the workflow produces a result.
/// </summary>
public class WorkflowCommand
{
    private readonly ILogger _logger;
    private readonly IActivity _activity;
    private readonly IVisualizationStrategy _visualizationStrategy;

    /// <inheritdoc cref="WorkflowCommand"/>
    public WorkflowCommand(ILogger logger, IActivity activity, IVisualizationStrategy visualizationStrategy)
    {
        _logger = logger;
        _activity = activity;
        _visualizationStrategy = visualizationStrategy;
    }

    /// <summary>
    /// The method called each time a <see cref="IAsset"/> is produced by the workflow.
    /// </summary>
    public void OnResult(object outputs)
    {
        if (((dynamic)outputs).Model is not Model model)
        {
            _logger.LogWarning("Activity output was not a model.");
            return;
        }
        VisualizeRequest request = new()
        {
            Model = model

            // TODO: Implement these
            // Name = _activity.Name,
            // Description = $"Executed {_activity.Name}."
        };
        _visualizationStrategy.Execute(request);
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
