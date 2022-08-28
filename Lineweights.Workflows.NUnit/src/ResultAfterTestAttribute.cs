using System.Text;
using Lineweights.Workflows.Containers;
using Lineweights.Workflows.Results;
using NUnit.Framework.Interfaces;
using StudioLE.Core.System;

namespace Lineweights.Workflows.NUnit;

/// <summary>
/// Execute a <see cref="ResultModel"/> after a test has run.
/// </summary>
//[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Assembly)]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public abstract class ResultAfterTestAttribute : Attribute, ITestAction
{
    /// <summary>
    /// Should the strategy be executed.
    /// </summary>
    public bool IsEnabled { get; set; } = AssemblyHelpers.IsDebugBuild();

    /// <inheritdoc/>
    public ActionTargets Targets => ActionTargets.Default;

    /// <inheritdoc cref="IResultStrategy"/>
    public abstract IResultStrategy? Strategy { get; }

    /// <inheritdoc/>
    public void BeforeTest(ITest test)
    {
    }

    /// <inheritdoc/>
    public void AfterTest(ITest test)
    {
        if (Strategy is null || !IsEnabled)
            return;
        if (test.Fixture is not ResultModel fixture)
            throw new($"{nameof(ResultAfterTestAttribute)} should only be used in the context of {nameof(ResultModel)}.");

        TestContext.ResultAdapter results = TestContext.CurrentContext.Result;
        StringBuilder name = new();

        name.Append($"{test.TestType} {test.FullName} (");

        if (results.FailCount == 0)
            name.Append($"All {results.PassCount} passed");
        else if (results.PassCount == 0)
            name.Append($"All {results.FailCount} failed");
        else
            name.Append($"{results.FailCount} of {results.PassCount + results.FailCount} failed");

        if (results.SkipCount > 0)
            name.Append($", {results.SkipCount} skipped");

        name.Append(")");

        DocumentInformation doc = new()
        {
            Name = name.ToString(),
            Description = string.Join(Environment.NewLine, test.Tests.Select(x => x.Name))
        };

        Container? container = Strategy?.Execute(fixture.Model, doc).Result;
    }
}
