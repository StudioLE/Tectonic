using System.Text;
using Lineweights.Core.Documents;
using Lineweights.Workflows.Hosting;
using Lineweights.Workflows.NUnit.Execution;
using Lineweights.Workflows.Visualization;
using Microsoft.Extensions.DependencyInjection;
using StudioLE.Core.System;

namespace Lineweights.Workflows.NUnit.Visualization;

/// <summary>
/// Execute an <see cref="IVisualizationStrategy"/> after a test has run.
/// By default the strategy is only run when the assembly is a DEBUG build.
/// The strategy is executed with the Model field or property on the test fixture.
/// First it looks for a public or private field named _model then a public property named Model.
/// </summary>
public class Visualize
{
    /// <inheritdoc cref="IVisualizationStrategy" />
    internal readonly IVisualizationStrategy _strategy;

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
    public async Task Execute(Model model)
    {
        if (!NUnitActivityFactory.IsExecuting && !IsEnabled)
            return;
        DocumentInformation doc = new()
        {
            Name = CreateSummary()
        };

        if (NUnitActivityFactory.IsExecuting)
        {
            NUnitActivityFactory.TestOutput = new
            {
                Model = model,
                Results = TestContext.CurrentContext.Result
            };
            return;
        }
        await _strategy.Execute(model, doc);
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
