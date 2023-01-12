using System.Collections.ObjectModel;
using System.Text;
using Geometrician.Core.Visualization;
using Lineweights.Core.Assets;
using Lineweights.Diagnostics.Hosting;
using Lineweights.Diagnostics.NUnit.Execution;
using Microsoft.Extensions.DependencyInjection;
using StudioLE.Core.System;

namespace Lineweights.Diagnostics.NUnit.Visualization;

/// <summary>
/// Execute an <see cref="IVisualizationStrategy"/> after a test has run.
/// By default the strategy is only run when the assembly is a DEBUG build.
/// The strategy is executed with the Model field or property on the test fixture.
/// First it looks for a public or private field named _model then a public property named Model.
/// </summary>
public class Visualize
{
    internal readonly IVisualizationStrategy _strategy;
    private readonly Collection<VisualizeRequest> _requests = new();

    /// <summary>
    /// Should the strategy be executed.
    /// </summary>
    public bool IsEnabled { get; set; } = AssemblyHelpers.IsDebugBuild();

    /// <inheritdoc cref="Visualize" />
    public Visualize(IVisualizationStrategy strategy)
    {
        _strategy = strategy;
    }

    /// <inheritdoc cref="Visualize" />
    public Visualize()
    {
        IServiceProvider services = Services.GetInstance();
        _strategy = services.GetRequiredService<IVisualizationStrategy>();
    }

    /// <inheritdoc cref="Visualize" />
    public void Queue(Model model, IReadOnlyCollection<IAsset>? assets = null)
    {
        if (!NUnitActivityFactory.IsExecuting && !IsEnabled)
            return;

        if (NUnitActivityFactory.IsExecuting)
        {
            NUnitActivityFactory.TestOutput = new
            {
                Model = model,
                Results = TestContext.CurrentContext.Result
            };
            return;
        }

        VisualizeRequest request = new()
        {
            Model = new(model.Transform, model.Elements)
        };
        _requests.Add(request);
    }

    /// <inheritdoc cref="Visualize" />
    public async Task Execute()
    {
        if (!NUnitActivityFactory.IsExecuting && !IsEnabled)
            return;
        await _strategy.Execute(_requests.ToArray());
    }

    private static string CreateSummary()
    {
        TestContext.TestAdapter test = TestContext.CurrentContext.Test;
        TestContext.ResultAdapter results = TestContext.CurrentContext.Result;
        StringBuilder builder = new();
        builder.Append($"{test.ID} {test.FullName} (");
        if (results.FailCount == 0)
            builder.Append($"All {results.PassCount} passed");
        else if (results.PassCount == 0)
            builder.Append($"All {results.FailCount} failed");
        else
            builder.Append($"{results.FailCount} of {results.PassCount + results.FailCount} failed");
        if (results.SkipCount > 0)
            builder.Append($", {results.SkipCount} skipped");
        builder.Append(")");
        return builder.ToString();
    }
}
