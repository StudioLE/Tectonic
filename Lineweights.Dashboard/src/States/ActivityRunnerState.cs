using System.IO;
using System.Reflection;
using Ardalis.Result;
using Lineweights.Workflows;
using Lineweights.Workflows.Execution;
using Lineweights.Workflows.Results;

namespace Lineweights.Dashboard.States;

/// <summary>
/// The SignalR connection state.
/// </summary>
/// <remarks>
/// Follows the <see href="https://stackoverflow.com/a/56223698/247218">state</see> pattern.
/// </remarks>
public class ActivityRunnerState
{
    private readonly ILogger<ActivityRunnerState> _logger;
    private readonly ResultsState _results;
    private readonly IActivityRunner _runner;

    /// <summary>
    /// The tests loaded from the _testRunner.
    /// </summary>
    public IReadOnlyCollection<string> Activities { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The currently selected test assembly.
    /// </summary>
    public string SelectedPackage { get; set; } = @"E:\Repos\Hypar\Lineweights\Lineweights.Workflows\samples\bin\Debug\netstandard2.0\Lineweights.Workflows.Samples.dll";

    /// <summary>
    /// The currently selected test.
    /// </summary>
    public string SelectedActivity { get; set; } = string.Empty;

    public ActivityRunnerState(ILogger<ActivityRunnerState> logger, ResultsState results, IActivityRunner runner)
    {
        _logger = logger;
        _results = results;
        _runner = runner;
    }

    /// <summary>
    /// Load the package.
    /// </summary>
    public void LoadPackage()
    {
        _logger.LogDebug($"{nameof(LoadPackage)} called on {SelectedPackage}.");

        Uri packageUri = new(SelectedPackage);
        if (!packageUri.IsFile)
        {
            _logger.LogError("Failed to extract activities. Uri was not a file.");
            return;
        }
        FileInfo dll = new(packageUri.AbsolutePath);
        if (!dll.Exists)
        {
            _logger.LogError("Failed to extract activities. File not found.");
            return;
        }

        // Load the assembly to ensure all dependencies are also loaded
        Assembly assembly = Assembly.LoadFrom(dll.FullName);

        Result<IReadOnlyCollection<string>> activities = _runner.ExtractActivities(assembly);
        if (!activities.IsSuccess)
        {
            _logger.LogError(string.Join(Environment.NewLine, activities.Errors.Prepend("Failed to extract activities.")));
            return;
        }

        Activities = activities.Value;
        SelectedActivity = Activities.First();
    }

    /// <summary>
    /// Run the selected activity.
    /// </summary>
    public void Execute()
    {
        _logger.LogDebug($"{nameof(Execute)}() called on {SelectedPackage} {SelectedActivity}.");

        _logger.LogDebug("Runner started.");
        Result<object> execution = _runner.Execute(SelectedActivity);
        _logger.LogDebug("Runner finished.");

        if (!execution.IsSuccess)
        {
            _logger.LogError(string.Join(Environment.NewLine, execution.Errors.Prepend("Failed to execute activity.")));
            return;
        }

        dynamic outputs = execution.Value;

        if (outputs.Model is not Model model)
        {
            _logger.LogWarning("Activity output was not a model.");
            return;
        }

        DocumentInformation doc = new()
        {
            Name = SelectedActivity,
            Description = $"Executed {SelectedActivity} from {SelectedPackage}."
        };
        Result result = ResultBuilder.Default(new BlobStorageStrategy(), model, doc);
        _results.Collection.Add(result);
    }
}
