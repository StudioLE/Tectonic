using System.Reflection;
using System.Text;
using Ardalis.Result;
using Lineweights.Workflows.Assets;
using Lineweights.Workflows.NUnit.Execution;
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
        if (!NUnitActivityFactory.IsExecuting && (Strategy is null || !IsEnabled))
            return;
        Result<Model> result = TryGetModel(test);
        DocumentInformation doc = new()
        {
            Name = CreateSummary(test, TestContext.CurrentContext.Result),
            Description = string.Join(Environment.NewLine, test.Tests.Select(x => x.Name))
        };

        if (NUnitActivityFactory.IsExecuting)
        {
            NUnitActivityFactory.TestOutput = new
            {
                Model = result.Value,
                Results = TestContext.CurrentContext.Result
            };
            return;
        }

        Model model = Validate.OrThrow(result);
        Task<Asset>? task = Strategy?.Execute(model, doc);
        Asset? asset = task?.Result;
    }

    private static Result<Model> TryGetModel(ITest test)
    {
        if(test.Fixture is null)
            return Result<Model>.Error("The test fixture was null.");
        Result<Model> field = test.Fixture.TryGetFieldValue<Model>("_model", BindingFlags.NonPublic | BindingFlags.Instance);
        if (field.IsSuccess)
            return field.Value;
        Result<Model> property = test.Fixture.TryGetPropertyValue<Model>("Model");
        if (property.IsSuccess)
            return property.Value;
        return Result<Model>.Error(field.Errors.Concat(property.Errors).Prepend("Failed to visualize after test.").Join());
    }

    private static string CreateSummary(ITest test, TestContext.ResultAdapter results)
    {
        StringBuilder builder = new();
        builder.Append($"{test.TestType} {test.FullName} (");
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
