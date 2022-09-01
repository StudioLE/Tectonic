using Lineweights.Workflows.Assets;
using Lineweights.Workflows.Visualization;
using NUnit.Framework.Interfaces;
using StudioLE.Core.System;

namespace Lineweights.Workflows.NUnit.Visualization;

/// <summary>
/// Execute an <see cref="IVisualizationStrategy"/> after a test has run.
/// By default the strategy is only run when the assembly is a DEBUG build.
/// The strategy is executed with the Model field or property on the test fixture.
/// First it looks for a public or private field named _model then a public property named Model.
/// </summary>
//[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Assembly)]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public abstract class VisualizeAfterTestAttribute : Attribute, ITestAction
{
    /// <summary>
    /// Should the strategy be executed.
    /// </summary>
    public bool IsEnabled { get; set; } = AssemblyHelpers.IsDebugBuild();

    /// <inheritdoc/>
    public ActionTargets Targets => ActionTargets.Default;

    /// <inheritdoc cref="IVisualizationStrategy"/>
    public abstract IVisualizationStrategy? Strategy { get; }

    /// <inheritdoc/>
    public void BeforeTest(ITest test)
    {
    }

    /// <inheritdoc/>
    public void AfterTest(ITest test)
    {
        if (Strategy is null || !IsEnabled)
            return;
        Model model = NUnitHelpers.GetModelOrThrow(test);
        DocumentInformation doc = new()
        {
            Name = NUnitHelpers.CreateSummary(test, TestContext.CurrentContext.Result),
            Description = string.Join(Environment.NewLine, test.Tests.Select(x => x.Name))
        };
        Task<Asset>? task = Strategy?.Execute(model, doc);
        Asset? asset = task?.Result;
    }
}
