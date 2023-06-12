namespace Cascade.Workflows.CommandLine.Tests.Resources;

public class ExampleThatThrowsActivity : IActivity<ExampleClass, ExampleClass>
{
    public Task<ExampleClass> Execute(ExampleClass example)
    {
        throw new("This activity intentionally throws an exception.");
    }
}
