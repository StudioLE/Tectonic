using Lineweights.Workflows.Results;
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
    private readonly IResultStrategy _resultStrategy;

    /// <inheritdoc cref="WorkflowCommand"/>
    public WorkflowCommand(ILogger logger, ActivityCommand activity, IResultStrategy resultStrategy)
    {
        _logger = logger;
        _activity = activity;
        _resultStrategy = resultStrategy;
    }

    /// <summary>
    /// The method called each time a <see cref="Result"/> is produced by the workflow.
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
        _resultStrategy.Execute(model, doc);
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
